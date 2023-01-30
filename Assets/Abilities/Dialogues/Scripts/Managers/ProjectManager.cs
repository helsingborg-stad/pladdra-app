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
        public GameObject pivotPrefab;
        [Header("Debug")]
        [GrayOut][SerializeField] Project currentProject;
        public Project Project { get => currentProject; }
        #endregion Public

        #region Scene References
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WebRequestHandler_Dialogues webRequestHandler { get { return transform.parent.gameObject.GetComponentInChildren<WebRequestHandler_Dialogues>(); } }
        public WebRequestHandler_Dialogues WebRequestHandler { get { return webRequestHandler; } }
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

            if (projects.ContainsKey(projectReference.name) && projects[projectReference.name].isLoadedAndInit)
            {
                Debug.Log($"ProjectManager: Project JSON {projectReference.name} is already loaded.");
                return;
            }

            StartCoroutine(webRequestHandler.LoadDialogueProject(projectReference.url, (Result result, string errors, Project project) =>
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
                        LoadProject(project);
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

            if (projects.ContainsKey(project.name) && projects[project.name].isLoadedAndInit)
            {
                Debug.Log($"ProjectManager: Project {project.name} is already loaded.");
                return;
            }

            Debug.Log("ProjectManager: Loading project data: " + project.name);
            // uiManager.ShowLoading("Laddar projekt " + project.name + "\n Titta dig omkring för att stabilisera AR!");
            StartCoroutine(webRequestHandler.LoadDialogueProjectResources(project, (Result result, string errors) =>
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
                projects.Add(project.name, project);
                project.InitProject(Origin(), uxManager, pivotPrefab);

                // Listens to Project.OnSaveProposal to save proposal to whatever backend is hooked up
                project.OnSaveProposal.AddListener((string proposalName, string proposalData) => OnSaveProposal.Invoke(proposalName, proposalData));

                if (project.marker != null) uxManager.ARReferenceImageHandler.AddReferenceImage(project.marker,project.name,1); //TODO Add width

                Debug.Log($"ProjectManager: Project {project.name} is loaded.");
            }));
        }

        /// <summary>
        /// Initializes a downloaded project, shows relevant UI, and adds relevant menu items
        /// </summary>
        public void ShowProject(string projectName)
        {
            // Remove all layers that have been added throughout the previous project.
            raycastManager.CleanLayerMasksFromlayersToRemove();

            if (this.currentProject != null && this.currentProject.isLoadedAndInit)
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