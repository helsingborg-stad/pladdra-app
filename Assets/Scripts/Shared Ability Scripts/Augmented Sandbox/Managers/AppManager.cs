using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UntoldGarden.AR;
using Pladdra.UI;
using Pladdra.Data;
using Pladdra.UX;
using System.Linq;

namespace Pladdra
{
    /// <summary>
    /// AppManager is the main manager for the app. 
    /// It handles the loading and showing of collections and project references.
    /// </summary>
    public class AppManager : MonoBehaviour, IDeepLinkHandler
    {
        #region Public
        [Header("URLs")]
        [Tooltip("Url to public projects, pointing directly to a wordpress post type.")]
        [SerializeField] protected string publicProjectsUrl = "";
        [Tooltip("Url to public collections, pointing directly to a wordpress post type containing collections. Collections need to follow same wordpress structure as dialoge-collections.")]
        [SerializeField] protected string collectionsUrl = "https://modul-test.helsingborg.io/augmented-sandbox/wp-json/wp/v2/dialogue-collections";
        [Tooltip("Url base for collection, replace {0} with collection post id.")]
        [SerializeField] protected string collectionUrlBase = "https://modul-test.helsingborg.io/augmented-sandbox/wp-json/wp/v2/collections/{0}?acf_format=standard";
        [Tooltip("Url base for project, replace {0} with project post id.")]
        [SerializeField] protected string projectUrlBase = "https://modul-test.helsingborg.io/augmented-sandbox/wp-json/wp/v2/dialogues/{0}?acf_format=standard";
        [Header("UI")]
        [Tooltip("Title shown above collection list.")]
        [SerializeField] protected string collectionListTitle;
        [Tooltip("Text shown below title.")]
        [SerializeField] protected string collectionListInfo;
        [Header("Scene References")]
        [SerializeField] protected ARSessionManager arSessionManager;
        public ARSessionManager ARSessionManager { get { return arSessionManager; } }
        #endregion Public

        #region Scene References
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected UXManager uxManager { get { return transform.parent.gameObject.GetComponentInChildren<UXManager>(); } }
        protected WebRequestHandler webRequestHandler { get { return transform.parent.gameObject.GetComponentInChildren<WebRequestHandler>(); } }
        #endregion Scene References

        #region Private
        List<ProjectReference> recentProjectList = new List<ProjectReference>();
        protected bool openingDeepLink = false;
        #endregion Private


        public void OpenDeepLink(string deeplink)
        {
            openingDeepLink = true;
            Debug.Log($"AppManager: OpenDeepLink: {deeplink}");
            if (deeplink.Contains("dialogues"))
            {
                // get the last part of deeplink after the final / in one line
                string projectId = deeplink.Split('/').Last();

                Debug.Log($"AppManager: OpenDeepLink: ProjectId: {projectId}");

                LoadProjectFromID(projectId);
            }
            else if (deeplink.Contains("collection"))
            {
                string collectionId = deeplink.Split('/').Last();
                Debug.Log($"AppManager: OpenDeepLink: CollectionId: {collectionId}");
                LoadProjectCollection(collectionId);
            }
        }

        #region Deeplink

        #endregion Deeplink

        #region Loading Collections

        /// <summary>
        /// Loads all ProjectCollections from collectionsUrl
        /// </summary>
        public void LoadProjectCollections()
        {
            StartCoroutine(webRequestHandler.LoadProjectCollections(collectionsUrl, (Result result, string errors, List<ProjectCollection> projectCollections) =>
                        {
                            switch (result)
                            {
                                case Result.PartialSuccess:
                                case Result.Failure:
                                    uiManager.ShowError("DownloadFailure", new string[] { collectionsUrl, errors });
                                    return;
                                case Result.Success:
                                    Debug.Log($"AppManager_Dialogues: Loaded DialogueCollections list with {projectCollections.Count} loaded successfully.");
                                    if (!openingDeepLink)
                                        DisplayProjectCollections(projectCollections);
                                    break;
                            }
                        }));
        }

        /// <summary>
        /// Loads a project collection from a collection id.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        public void LoadProjectCollection(string id)
        {
            LoadProjectListFromURL(
                string.Format(collectionUrlBase, id),
                projectUrlBase,
                () => { UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby"); }
                );
        }

        #endregion Loading Collections

        #region Loading Project lists

        /// <summary>
        /// Load all projects from a list of project ids.
        /// </summary>
        /// <param name="ids">Ids for projects to load.</param>
        protected void LoadProjectListFromIDs(int[] ids, string urlBase, Action onReturn, string collectionName, string collectionInfo)
        {
            string[] urls = new string[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                urls[i] = string.Format(urlBase, ids[i]);
            }

            StartCoroutine(webRequestHandler.LoadProjectsFromIDs(urls, urlBase, (Result result, string errors, List<ProjectReference> projects) =>
                        {
                            switch (result)
                            {
                                case Result.PartialSuccess:
                                case Result.Failure:
                                    uiManager.ShowError("DownloadFailure", new string[] { string.Join(", ", urls), errors });
                                    return;
                                case Result.Success:
                                    Debug.Log($"AppManager: Loaded {projects.Count} projects.");
                                    DisplayProjectList(projects, onReturn, collectionName, collectionInfo);
                                    break;
                            }
                        }));
        }

        /// <summary>
        /// Load all projects at a public url.
        /// </summary>
        /// <param name="ids">Ids for projects to load.</param>
        protected void LoadProjectListFromURL(string url, string urlBase, Action onClose)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(urlBase))
            {
                Debug.LogError("AppManager: LoadProjectListFromURL: url or urlBase is null or empty.");
                return;
            }
            StartCoroutine(webRequestHandler.LoadProjectsFromURL(url, urlBase, (Result result, string errors, List<ProjectReference> projects) =>
                        {
                            switch (result)
                            {
                                case Result.PartialSuccess:
                                case Result.Failure:
                                    uiManager.ShowError("DownloadFailure", new string[] { url, errors });
                                    return;
                                case Result.Success:
                                    Debug.Log($"AppManager: Project list with {projects.Count} loaded successfully.");
                                    DisplayProjectList(projects, onClose);
                                    break;
                            }
                        }));
        }

        #endregion Loading Project lists

        #region Loading Projects

        public void LoadProjectFromID(string id)
        {
            StartCoroutine(webRequestHandler.LoadProjectFromID(id, projectUrlBase, (Result result, string errors, ProjectReference project) =>
                        {
                            switch (result)
                            {
                                case Result.PartialSuccess:
                                case Result.Failure:
                                    uiManager.ShowError("DownloadFailure", new string[] { string.Format(projectUrlBase, id), errors });
                                    return;
                                case Result.Success:
                                    Debug.Log($"AppManager: Project {project.name} loaded successfully.");
                                    LoadProject(project);
                                    break;
                            }
                        }));
        }

        #endregion Loading Projects

        #region UI

        /// <summary>
        /// Displays a list of project collections.
        /// Close button returns user to the Lobby scene, as there is no layer above project collections.
        /// </summary>
        /// <param name="projectCollections">The list of project collections to show.</param>
        public void DisplayProjectCollections(List<ProjectCollection> projectCollections)
        {
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null");
                return;
            }

            if (projectCollections == null || projectCollections.Count == 0)
            {
                Debug.Log("ProjectCollections is null or empty");
                // TODO
                return;
            }

            Action[] actions = new Action[projectCollections.Count];
            for (int i = 0; i < projectCollections.Count; i++)
            {
                int index = i;
                actions[i] = () =>
                {
                    LoadProjectListFromIDs(
                        projectCollections[index].projectIds.ToArray(),
                        projectUrlBase,
                        () => { LoadProjectCollections(); },
                        projectCollections[index].name,
                        projectCollections[index].description);
                };
            }

            uiManager.DisplayUI("project-list", root =>
            {
                ListView listView = root.Q<ListView>("project-list");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.uiAssets.Find(x => x.name == "project-button-template").visualTreeAsset.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {
                                    element.Q<Button>("project-button").clicked += () => { actions[i](); };
                                    element.Q<Label>("name").text = projectCollections[i].name;
                                    element.Q<Label>("description").text = projectCollections[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = projectCollections;

                if (collectionListTitle != "") root.Q<Label>("title").text = collectionListTitle;
                if (collectionListInfo != "") root.Q<Label>("info").text = collectionListInfo;

                root.Q<Button>("close").clicked += () =>
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                };
            },
            true);
        }

        /// <summary>
        /// Displays a list of projects.
        /// Close button is specified by the caller.
        /// </summary>
        /// <param name="projects">The list of projects to show.</param>
        /// <param name="onClose">Action to invoke on when the user press the close button.</param>
        public void DisplayProjectList(List<ProjectReference> projects, Action onClose, string projectListTitle = "", string projectListInfo = "")
        {
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null");
                return;
            }

            if (projects == null || projects.Count == 0)
            {
                Debug.Log("Project list is null or empty, cannot display project list.");
                return;
            }

            recentProjectList = projects;

            Action[] actions = new Action[projects.Count];
            for (int i = 0; i < projects.Count; i++)
            {
                int index = i;
                ProjectReference project = projects[i];
                actions[i] = () =>
                {
                    LoadProject(project);
                };
            }

            uiManager.DisplayUI("project-list", root =>
            {
                ListView listView = root.Q<ListView>("project-list");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.uiAssets.Find(x => x.name == "project-button-template").visualTreeAsset.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {
                                    if (IsProjectActive(projects[i]))
                                    {
                                        element.Q<Button>("project-button").SetEnabled(false);
                                    }
                                    else
                                    {
                                        element.Q<Button>("project-button").clicked += () => { actions[i](); };
                                    }
                                    element.Q<Label>("name").text = projects[i].name;
                                    element.Q<Label>("description").text = projects[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = projects;

                if (projectListTitle != "") root.Q<Label>("title").text = projectListTitle;
                if (projectListInfo != "") root.Q<Label>("info").text = projectListInfo;

                root.Q<Button>("close").clicked += () =>
                {
                    onClose();
                };
            },
            true);
        }

        public void DisplayRecentProjectList(Action onClose)
        {
            DisplayProjectList(recentProjectList, onClose);
        }

        #endregion UI

        /// <summary>
        /// Base class to load the project. Specific loading logic should be implemented in derived classes as it differs from ability type.
        /// </summary>
        /// <param name="project">The reference to the project to load.</param>
        protected virtual void LoadProject(ProjectReference project)
        {
        }

        /// <summary>
        /// Base class to check if the project is active, and if it should be disabled in the project list UI.
        /// Specific logic should be implemented in derived classes as it differs from ability type.
        /// </summary>
        /// <param name="project">The reference to the project to check.</param>

        protected virtual bool IsProjectActive(ProjectReference project)
        {
            return false;
        }
    }
}