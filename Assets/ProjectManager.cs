using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Workspace;
using UntoldGarden;
using Pladdra.UI;
using Pladdra.Data;
using UnityEngine.UIElements;
using System.Linq;

namespace Pladdra
{
    enum JSONSchema { Project, List }
    [RequireComponent(typeof(WebRequestHandler))]
    public class ProjectManager : MonoBehaviour
    {
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return GetComponent<WebRequestHandler>(); } }
        protected InteractionManager interactionManager { get { return transform.parent.gameObject.GetComponentInChildren<InteractionManager>(); } }
        protected Transform origin;
        [Header("Debug")]
        [GrayOut] protected Project project;
        protected List<Project> recentProjects = new List<Project>();

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
            Debug.Log(string.Format("ProjectManager: Loading project {0}", project.Name));
            uiManager.ShowLoading("Loading project " + project.Name);
            StartCoroutine(webRequestHandler.LoadProjectResources(project, (Result result, string errors) =>
            {
                switch (result)
                {
                    case Result.Failure:
                        Debug.LogError($"ProjectManager: Project {project.Name} failed to load data. {errors}");
                        uiManager.ShowError("DownloadFailure", new string[] { project.Name, errors });
                        return;
                    case Result.PartialSuccess:
                        Debug.Log($"ProjectManager: Project {project.Name} loaded most data. \n {errors}");
                        uiManager.ShowError("DownloadPartialSuccess", new string[] { project.Name, errors });
                        break;
                    case Result.Success:
                        Debug.Log($"ProjectManager: Project {project.Name} loaded successfully.");
                        break;
                }
                project.CreateThumbnails();
                InitProject();
            }));
        }

        /// <summary>
        /// Initializes a downloaded project and shows relevant UI.
        /// </summary>
        public void InitProject()
        {
            Debug.Log("Init Project " + project.Name);
            if (project.markerRequired)
            {
                Debug.Log("Project requires marker");
                uiManager.ShowUI("look-for-marker");
                // TODO Hook up listeners to marker found event
            }
            else
            {
                Dictionary<string, Action> actions = new Dictionary<string, Action>();
                if (project.UserCanInteract())
                {
                    actions.Add("Start building", () => { ShowWorkspace(); });
                }
                if (project.HasProposals())
                {
                    actions.Add("View proposals", () => { ShowProposals(); });
                }
                uiManager.ShowUI("project-info", root =>
                        {
                            root.Q<Label>("Name").text = project.Name;
                            root.Q<Label>("Description").text = project.Description;
                            ListView buttons = root.Q<ListView>("Buttons");
                            if (actions.Count > 0)
                            {
                                buttons.makeItem = () =>
                                {
                                    var button = uiManager.buttonTemplate.Instantiate();
                                    return button;
                                };
                                buttons.bindItem = (element, i) =>
                                {
                                    Button button = element.Q<Button>();
                                    button.text = actions.Keys.ToArray()[i];
                                    button.clicked += () => { actions[actions.Keys.ToArray()[i]](); };
                                };
                                buttons.fixedItemHeight = 50;
                                buttons.itemsSource = actions.Keys.ToArray();
                            }
                            else
                            {
                                buttons.visible = false;
                                buttons.style.height = 0;
                            }
                        }
            );
                InstantiateProject();
            }
        }

        public void InstantiateProject()
        {
            project.InstantiateStaticResources(Origin());

        }

        public void ShowProposals()
        {
            Debug.Log("Show proposals");
            // project.ShowUserProposals(); // TODO
        }

        public void HideProposals()
        {
            project.HideUserProposals();
        }
        public void ShowWorkspace()
        {
            Debug.Log("Show Workspace");
            interactionManager.Project = project;
            interactionManager.ShowWorkspaceDefault();
            // TODO
        }

        public void HideWorkspace()
        {
            // TODO
        }

        // TODO allow user defined origin
        // TODO allow different offsets 
        /// <summary>
        /// Global origin of the project. Defined either as a spot on the floor in front of the user or the location of the project AR marker.
        /// </summary>
        /// <returns>Global origin of the project.</returns>
        public Transform Origin()
        {
            if (origin == null)
            {
                origin = new GameObject("ProjectOrigin").transform;
                origin.position = Camera.main.gameObject.RelativeToObject(new Vector3(0, 0, 4), VectorExtensions.RelativeToObjectOptions.OnGroundLayers, new string[] { "ARMesh" });
            }
            return origin;
        }

    }
}