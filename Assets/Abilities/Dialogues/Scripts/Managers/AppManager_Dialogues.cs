using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Data;
using Pladdra.DialogueAbility.Data;

namespace Pladdra.DialogueAbility
{
    public class AppManager_Dialogues : Pladdra.AppManager
    {
        #region Private
        protected ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }

        #endregion Private

        void Start()
        {
            if (!openingDeepLink)
                LoadProjectCollections();
        }
        protected override void LoadProject(ProjectReference project)
        {
            // projectManager.ClearProject();
            projectManager.LoadProjectJSON(project);
            StartCoroutine(this.WaitToShowProject(project.name));
        }

        /// <summary>
        /// Coroutine that waits until we have an AR plane, the project is created, and the project is loaded
        /// </summary>
        /// <returns>Calls projectManager.InitProject when all criteria is met</returns>
        IEnumerator WaitToShowProject(string project)
        {
            yield return null;
            float f = 0;
            while (!arSessionManager.HasARPlane() || !projectManager.Projects.ContainsKey(project))// || !projectManager.Projects[project].isLoadedAndInit)
            {
                if (f > 10)
                {
                    Debug.Log($"Waiting for project {project} to be loaded. \n ARPlane: {arSessionManager.HasARPlane()}, Project: {(projectManager.Projects != null ? projectManager.Projects.ContainsKey(project) : "No projects list.")}, project count is {projectManager.Projects.Count}");
                    f = 0;
                }
                f += Time.deltaTime;
                yield return null;
            }
            projectManager.ShowProject(project);
        }

        protected override bool IsProjectActive(ProjectReference project)
        {
            return projectManager.Project != null && project.id == projectManager.Project.id;
        }
    }
}