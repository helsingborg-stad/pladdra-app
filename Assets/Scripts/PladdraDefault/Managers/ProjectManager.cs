using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Workspace;
using UntoldGarden;
using Pladdra.DefaultAbility;
using Pladdra.DefaultAbility.UI;
using Pladdra.DefaultAbility.Data;
using UnityEngine.UIElements;
using System.Linq;
using Pladdra.DefaultAbility.UX;

namespace Pladdra
{
    enum JSONSchema { Project, List }
    [RequireComponent(typeof(WebRequestHandler))]
    public class ProjectManager : MonoBehaviour
    {
        #region Debug
        [Header("Debug")]
        [GrayOut] protected Project project;
        public Project Project { get => project; }
        #endregion Debug

        #region Private
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return GetComponent<WebRequestHandler>(); } }
        protected ProposalManager proposalManager { get { return transform.parent.gameObject.GetComponentInChildren<ProposalManager>(); } }
        protected InteractionManager interactionManager { get { return transform.parent.gameObject.GetComponentInChildren<InteractionManager>(); } }
        protected ViewingModeManager viewingModeManager { get { return transform.parent.gameObject.GetComponentInChildren<ViewingModeManager>(); } }
        protected Transform origin;
        protected List<Project> recentProjects = new List<Project>();

        #endregion Private

        /// <summary>
        /// Downloads all data for a project.
        /// </summary>
        /// <param name="project">The project to download.</param>
        public void LoadProject(Project project)
        {
            if (this.project != null)
            {
                this.project.Hide();
                recentProjects.Add(this.project);
            }
            this.project = project;
            Debug.Log(string.Format("ProjectManager: Loading project {0}", project.name));
            uiManager.ShowLoading("Loading project " + project.name);
            StartCoroutine(webRequestHandler.LoadProjectResources(project, (Result result, string errors) =>
            {
                switch (result)
                {
                    case Result.Failure:
                        Debug.LogError($"ProjectManager: Project {project.name} failed to load data. {errors}");
                        uiManager.ShowError("DownloadFailure", new string[] { project.name, errors });
                        return;
                    case Result.PartialSuccess:
                        Debug.Log($"ProjectManager: Project {project.name} loaded most data. \n {errors}");
                        uiManager.ShowError("DownloadPartialSuccess", new string[] { project.name, errors });
                        break;
                    case Result.Success:
                        Debug.Log($"ProjectManager: Project {project.name} loaded successfully.");
                        break;
                }
                project.CreateThumbnails();
                proposalManager.LoadLocalProsals(); // TODO Load from server too
                project.isLoaded = true;
            }));
        }

        /// <summary>
        /// Initializes a downloaded project, shows relevant UI, and adds relevant menu items
        /// </summary>
        public void InitProject()
        {
            interactionManager.Project = project;
            project.InitProject(Origin(), proposalManager, interactionManager);
            project.InstantiateStaticResources();

            UXHandler ux = new AllowUserToViewProject(interactionManager);
            interactionManager.UseUxHandler(ux);

            uiManager.MenuManager.AddMenuItem(new MenuItem()
            {
                id = "zen-mode",
                name = "Zenmode", // TODO Change to call from UItexts
                action = () =>
                {
                    UXHandler ux = new AllowUserToViewZenMode(interactionManager);
                    interactionManager.UseUxHandler(ux);
                }
            });
            uiManager.MenuManager.AddMenuItem(new MenuItem()
            {
                id = "white-mode",
                name = "Whitemode", // TODO Change to call from UItexts
                action = () =>
                {
                    viewingModeManager.ToggleWhiteSphere();
                }
            });
        }

        public Transform Origin()
        {
            if (origin == null) origin = new GameObject("ProjectOrigin").transform;

            return origin;
        }

    }
}