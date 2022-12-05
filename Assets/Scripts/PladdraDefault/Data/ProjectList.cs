using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    /// <summary>
    /// List of project references.
    /// </summary>
    [System.Serializable]
    public class ProjectList
    {
        public List<ProjectLink> projects;
        public ProjectList(List<ProjectLink> projects)
        {
            this.projects = projects;
        }
    }
}