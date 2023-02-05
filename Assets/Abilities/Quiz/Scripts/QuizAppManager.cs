using System.Collections;
using System.Collections.Generic;
using Pladdra.Data;
using UnityEngine;

namespace Pladdra.ARSandbox.Quizzes
{
    public class QuizAppManager : AppManager
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
        }
    }
}