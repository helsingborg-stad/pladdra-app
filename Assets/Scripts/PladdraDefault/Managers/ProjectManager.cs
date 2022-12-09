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
    /// <summary>
    /// Manages project data: loading, unloading, switching project, etc.
    /// </summary>
    [RequireComponent(typeof(WebRequestHandler))]
    public class ProjectManager : MonoBehaviour
    {
        #region Debug
        [Header("Debug")]
        [GrayOut] protected Project currentProject;
        public Project Project { get => currentProject; }
        #endregion Debug

        #region Private
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return GetComponent<WebRequestHandler>(); } }
        protected ProposalManager proposalManager { get { return transform.parent.gameObject.GetComponentInChildren<ProposalManager>(); } }
        protected UXManager uxManager { get { return transform.parent.gameObject.GetComponentInChildren<UXManager>(); } }
        protected ViewingModeManager viewingModeManager { get { return transform.parent.gameObject.GetComponentInChildren<ViewingModeManager>(); } }
        protected Transform origin;
        protected Dictionary<string, Project> projects = new Dictionary<string, Project>();
        public Dictionary<string, Project> Projects { get => projects; }

        #endregion Private

        /// <summary>
        /// Downloads the JSON with project data from wordpress.
        /// </summary>
        /// <param name="projectReference">The project link containing name and url</param>
        public void LoadProjectJSON(ProjectReference projectReference)
        {
            StartCoroutine(webRequestHandler.LoadProjectJSON(projectReference.url, (Result result, string errors, WordpressData wordpressData) =>
            {
                switch (result)
                {
                    case Result.PartialSuccess:
                    case Result.Failure:
                        Debug.LogError($"ProjectManager: Project {projectReference.name} failed to load data. {errors}");
                        uiManager.ShowError("DownloadFailure", new string[] { projectReference.name, errors });
                        return;
                    case Result.Success:
                        Debug.Log($"ProjectManager: Project {projectReference.name} loaded successfully.");
                        // CreateProjectFromWordpressData(wordpressData);
                        LoadProject(wordpressData.MakeProject());
                        break;
                }
            }));
        }
        
        /// <summary>
        /// Downloads all data for a project.
        /// </summary>
        /// <param name="project">The project to download.</param>
        public void LoadProject(Project project)
        {
            if (project == null)
            {
                Debug.LogError("ProjectManager: Project is null.");
                return;
            }

            if (projects.ContainsKey(project.name) && projects[project.name].isLoaded)
            {
                Debug.Log($"ProjectManager: Project {project.name} is already loaded.");
                return;
            }

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
                        Debug.Log($"ProjectManager: Project {project.name} loaded some data. \n {errors}");
                        uiManager.ShowError("DownloadPartialSuccess", new string[] { project.name, errors });
                        break;
                    case Result.Success:
                        Debug.Log($"ProjectManager: Project {project.name} loaded successfully.");
                        break;
                }
                proposalManager.LoadLocalProsals(project); // TODO Move this to the project object 
                project.InitProject(Origin(), proposalManager, uxManager);
                projects.Add(project.name, project);

                Debug.Log($"ProjectManager: Project {project.name} is loaded.");
            }));
        }

        /// <summary>
        /// Initializes a downloaded project, shows relevant UI, and adds relevant menu items
        /// </summary>
        public void ShowProject(string projectName)
        {
            if (this.currentProject != null)
            {
                this.currentProject.Hide();
                proposalManager.HideAllProposals(); // TODO Move this to the project object 
            }

            this.currentProject = projects[projectName];

            uxManager.Project = currentProject;
            currentProject.ShowStaticResources();

            UXHandler ux = new AllowUserToViewProject(uxManager);
            uxManager.UseUxHandler(ux);
        }

        /// <summary>
        /// Transform that contains all project objects.
        /// </summary>
        /// <returns>The origin transform.</returns>
        public Transform Origin()
        {
            if (origin == null) origin = new GameObject("ProjectOrigin").transform;
            return origin;
        }

    }
}