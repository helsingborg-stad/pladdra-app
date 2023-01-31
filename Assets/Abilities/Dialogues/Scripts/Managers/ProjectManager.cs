using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.UI;
using Pladdra.Data;
using Pladdra.DialogueAbility.Data;
using Pladdra.DialogueAbility.UX;
using UnityEngine.Events;
using Pladdra.UX;

namespace Pladdra.DialogueAbility
{
    /// <summary>
    /// Manages project data: loading, unloading, and switching projects.
    /// </summary>
    public class ProjectManager : MonoBehaviour
    {
        #region Public
        [Header("Debug")]
        [GrayOut][SerializeField] Project currentProject = null;
        public Project Project { get => currentProject; }
        #endregion Public

        #region Scene References
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return transform.parent.gameObject.GetComponentInChildren<WebRequestHandler>(); } }
        public WebRequestHandler WebRequestHandler { get { return webRequestHandler; } }
        protected UXManager uxManager { get { return transform.parent.gameObject.GetComponentInChildren<UXManager>(); } }
        protected ViewingModeManager viewingModeManager { get { return transform.parent.gameObject.GetComponentInChildren<ViewingModeManager>(); } }
        protected RaycastManager raycastManager { get { return transform.parent.gameObject.GetComponentInChildren<RaycastManager>(); } }
        protected Transform origin;
        #endregion Scene References

        #region Private
        protected Dictionary<string, Project> projects = new Dictionary<string, Project>();
        public Dictionary<string, Project> Projects { get => projects; }
        public UnityEvent<string, string> OnSaveProposal = new UnityEvent<string, string>();
        public UnityEvent OnOpenProject = new UnityEvent();
        #endregion Private

        /// <summary>
        /// Downloads the JSON with project data from wordpress.
        /// </summary>
        /// <param name="projectReference">The project link containing name and url</param>
        public void LoadProjectJSON(ProjectReference projectReference)
        {
            Debug.Log($"ProjectManager: Loading project JSON {projectReference.name} from {projectReference.url}");

            uiManager.ShowLoading("Laddar projekt " + projectReference.name + "\n Titta dig omkring för att stabilisera AR!");

            if (projects.ContainsKey(projectReference.name))// && projects[projectReference.name].isLoadedAndInit)
            {
                Debug.Log($"ProjectManager: Project JSON {projectReference.name} is already loaded.");
                return;
            }

            StartCoroutine(webRequestHandler.LoadText(projectReference.url, (Result result, string errors, string json) =>
            {
                switch (result)
                {
                    case Result.PartialSuccess:
                    case Result.Failure:
                        Debug.LogError($"ProjectManager: Project {projectReference.name} failed to load data. {errors}");
                        uiManager.ShowError("DownloadFailure", new string[] { projectReference.name, errors });
                        return;
                    case Result.Success:
                        Debug.Log($"ProjectManager: Project {projectReference.name} JSON loaded successfully.");
                        // Debug.Log($"Downloaded project JSON: {request.downloadHandler.text}");
                        // WordpressData_Dialogues wordpressData = JsonUtility.FromJson<WordpressData_Dialogues>(json);
                        LoadProject(JsonUtility.FromJson<WordpressData_Dialogues>(json).MakeProject());
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

            if (projects.ContainsKey(project.name))// && projects[project.name].isLoadedAndInit)
            {
                Debug.Log($"ProjectManager: Project {project.name} is already loaded.");
                return;
            }

            Debug.Log("ProjectManager: Loading project data: " + project.name);
            //TODO This should probably move to the project class, and be done after geospatial implementation
            List<(string name, string url)> resourcesToLoad = new List<(string name, string url)>();
            foreach (var resource in project.resources)
            {
                resourcesToLoad.Add((resource.name, resource.url));
            }

            StartCoroutine(webRequestHandler.LoadFiles(resourcesToLoad, (Result result, string errors, List<(string name, string path)> files) =>
            {
                switch (result)
                {
                    case Result.Failure:
                        Debug.LogError($"ProjectManager: Project {project.name} failed to load all data. {errors}");
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
                foreach (var file in files)
                {
                    project.resources.Find(x => x.name == file.name).path = file.path;
                }

                projects.Add(project.name, project);
                project.Init(Origin(), uxManager);

                // Listens to Project.OnSaveProposal to save proposal to whatever backend is hooked up
                project.OnSaveProposal.AddListener((string proposalName, string proposalData) => OnSaveProposal.Invoke(proposalName, proposalData));

                // if (project.marker.image != null) uxManager.ARReferenceImageHandler.AddReferenceImage(project.marker.image, project.name, project.marker.width);

                Debug.Log($"ProjectManager: Project {project.name} is loaded.");
            }));
        }

        /// <summary>
        /// Initializes a downloaded project, shows relevant UI, and adds relevant menu items
        /// </summary>
        public void ShowProject(string projectName)
        {
            Debug.Log($"ProjectManager: Showing project {projectName}");
            // Remove all layers that have been added throughout the previous project.
            raycastManager.CleanLayerMasksFromlayersToRemove();

            if (this.currentProject != null && this.currentProject.isCreated)
            {
                this.currentProject.Hide();
                this.currentProject.HideProposals();
            }

            this.currentProject = projects[projectName];
            uxManager.Project = currentProject;

            UXHandler ux = new AllowUserToViewProject(uxManager);
            uxManager.UseUxHandler(ux);
        }

        // public void SaveProposal(string name, string json)
        // {
        //     OnSaveProposal.Invoke(name, json);
        // }

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