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
    public class AppManager : MonoBehaviour
    {
        public PladdraDefaultSettings settings;
        public ARSessionManager arSessionManager;
        List<ProjectLink> publicProjects;
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }
        bool afterStart;

        //TODO Load unsaved proposal at start
        void Awake()
        {
            Application.deepLinkActivated += url => OpenDeeplink(url);
        }
        void Start()
        {
            // TODO Download public projects from server and populate publicProjects
            afterStart = true;

            if (settings.useTestProjects)
            {
                ShowTestProjectList();
            }
            else
            {
                ShowProjectList(new ProjectList(publicProjects));
            }
        }

        public void ShowTestProjectList()
        {
            if (uiManager == null)
            {
                Debug.Log("UIManager is null");
                return;
            }

            Action[] actions = new Action[settings.testProjects.projects.Count];
            for (int i = 0; i < settings.testProjects.projects.Count; i++)
            {
                int index = i;
                LocalTestProjectLink project = settings.testProjects.projects[i];
                actions[i] = () =>
                {
                    projectManager.LoadProject(JsonUtility.FromJson<Project>(project.json.ToString()));
                    StartCoroutine(this.WaitToInitProject());
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
                                    element.Q<Label>("name").text = settings.testProjects.projects[i].name;
                                    element.Q<Label>("description").text = settings.testProjects.projects[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = settings.testProjects.projects;
            });
        }

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
            uiManager.ShowUI("project-list", root =>
            {

            });
        }

        /// <summary>
        /// Coroutine to wait until we have localized AR planes and the project is loaded with creating it
        /// </summary>
        /// <returns>Calls projectManager.InitProject when all criteria is met</returns>
        IEnumerator WaitToInitProject()
        {
            Debug.Log("WaitToInitProject");
            yield return null;
            while (!arSessionManager.HasARPlane() || !projectManager.Project.isLoaded)
            {
                yield return null;
            }
            projectManager.InitProject();
        }

        void OpenDeeplink(string url)
        {
            // TODO Ask user to save if they have unsaved changes from previus project?
            if (afterStart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // TODO Open deeplink
        }

    }
}