using System.Collections;
using System.Collections.Generic;
using Pladdra.Data;
using UnityEngine;

namespace Pladdra.QuizAbility
{
    public class AppManager_Quiz : Pladdra.AppManager
    {

        [Tooltip("Title shown above project list.")]
        [SerializeField] protected string quizListTitle;
        [Tooltip("Text shown below title.")]
        [SerializeField] protected string quizListInfo;
        protected QuizManager quizManager { get { return transform.parent.gameObject.GetComponentInChildren<QuizManager>(); } }

        void Start()
        {
            LoadProjectListFromURL(
                publicProjectsUrl, 
                projectUrlBase, 
                () => {UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");});
        }

        /// <summary>
        /// Loads a quiz project
        /// </summary>
        /// <param name="quizProject">Quiz to load.</param> 
        protected override void LoadProject(ProjectReference quizProject)
        {
            Debug.Log($"Loading project {quizProject.name}");
            quizManager.LoadQuizCollection(quizProject);
            // StartCoroutine(WaitToShowProject(quizProject.name));
        }

        /// <summary>
        /// Coroutine that waits until we have an AR plane, the project is created, and the project is loaded
        /// </summary>
        /// <returns>Calls projectManager.InitProject when all criteria is met</returns>
        // IEnumerator WaitToShowProject(string project)
        // {
        //     yield return null;
        //     float f = 0;
        //     while (!arSessionManager.HasARPlane() || !quizManager.Quizzes.ContainsKey(project) || !quizManager.Quizzes[project].isLoadedAndInit)
        //     {
        //         if (f > 10)
        //         {
        //             Debug.Log($"Waiting for project {project} to be loaded. \n ARPlane: {arSessionManager.HasARPlane()}, Project: {(quizManager.Quizzes != null ? quizManager.Quizzes.ContainsKey(project) : "No projects list.")}, Loaded: {(quizManager.Quizzes.ContainsKey(project) ? quizManager.Quizzes[project].isLoadedAndInit : "No project")}");
        //             f = 0;
        //         }
        //         f += Time.deltaTime;
        //         yield return null;
        //     }
        //     quizManager.ShowQuizCollection();
        // }
    }
}