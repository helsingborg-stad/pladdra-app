using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.DefaultAbility.Data;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Pladdra.DefaultAbility.UI;
using System;
using UntoldGarden.AR;

namespace Pladdra.DefaultAbility
{
    //TODO This one is a bit of a mess atm due to the need to have local json files for testing
    public class AppManager : MonoBehaviour
    {
        #region Public
        public PladdraDefaultSettings settings;
        [SerializeField] ARSessionManager arSessionManager;
        #endregion Public

        #region Private
        List<ProjectReference> publicProjects;
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }
        bool afterStart;
        #endregion Private

        // TODO This will move to the Pladdra lobby
        void Awake()
        {
            Application.deepLinkActivated += url => OpenDeeplink(url);
        }

        void Start()
        {
            // TODO Download public projects list from wordpress and populate publicProjects
            afterStart = true;

            //TODO Remove the test projects functionality
            if (settings.useTestProjects)
            {
                ShowTestProjectList();
            }
            else
            {
                Debug.Log("hi");
                //TODO ProjectList should be taken from wordpress
                ShowProjectList(settings.projectList);
            }
        }

        //TODO Quick fix that shows the projects from the local json file 
        public void ShowTestProjectList()
        {
            if (uiManager == null)
            {
                Debug.Log("UIManager is null");
                return;
            }

            Action[] actions = new Action[settings.localProjectList.projects.Count];
            for (int i = 0; i < settings.localProjectList.projects.Count; i++)
            {
                int index = i;
                LocalTestProjectReference project = settings.localProjectList.projects[i];
                actions[i] = () =>
                {
                    projectManager.LoadProject(JsonUtility.FromJson<Project>(project.json.ToString()));
                    StartCoroutine(this.WaitToShowProject(project.name));
                };
            }

            uiManager.ShowUI("project-list", root =>
            {
                ListView listView = root.Q<ListView>("project-list");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.projectButtonTemplate.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {
                                    element.Q<Button>("project-button").clicked += () => { actions[i](); };
                                    element.Q<Label>("name").text = settings.localProjectList.projects[i].name;
                                    element.Q<Label>("description").text = settings.localProjectList.projects[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = settings.localProjectList.projects;
            });
        }

        //TODO
        public void ShowProjectList(ProjectList projectList)
        {
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null");
                return;
            }
            if (projectList.projects == null || projectList.projects.Count == 0)
            {
                Debug.Log("ProjectList is null or empty");
                // TODO
                return;
            }

            Action[] actions = new Action[projectList.projects.Count];
            for (int i = 0; i < projectList.projects.Count; i++)
            {
                int index = i;
                ProjectReference project = projectList.projects[i];
                actions[i] = () =>
                {
                    projectManager.LoadProjectJSON(project);
                    StartCoroutine(this.WaitToShowProject(project.name));
                };
            }

            uiManager.ShowUI("project-list", root =>
            {
                ListView listView = root.Q<ListView>("project-list");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.projectButtonTemplate.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {
                                    element.Q<Button>("project-button").clicked += () => { actions[i](); };
                                    element.Q<Label>("name").text = settings.projectList.projects[i].name;
                                    element.Q<Label>("description").text = settings.projectList.projects[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = settings.projectList.projects;
            });
        }

        /// <summary>
        /// Coroutine that waits until we have an AR plane, the project is created, and the project is loaded
        /// </summary>
        /// <returns>Calls projectManager.InitProject when all criteria is met</returns>
        IEnumerator WaitToShowProject(string project)
        {
            yield return null;
            while (!arSessionManager.HasARPlane() || !projectManager.Projects.ContainsKey(project) || !projectManager.Projects[project].isLoaded)
            {
                // Debug.Log($"Waiting for project {project} to be loaded. \n ARPlane: {arSessionManager.HasARPlane()}, Project: {(projectManager.Projects != null ? projectManager.Projects.ContainsKey(project) : "No projects list.")}, Loaded: {(projectManager.Projects.ContainsKey(project) ? projectManager.Projects[project].isLoaded : "No project")}");
                yield return null;
            }
            projectManager.ShowProject(project);
        }

        /// <summary>
        /// Opens a project from a deeplink
        /// </summary>
        /// <param name="url"></param>
        void OpenDeeplink(string url)
        {
            // TODO Ask user to save if they have unsaved changes from previus project
            if (afterStart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // TODO Open deeplink
        }

    }
}