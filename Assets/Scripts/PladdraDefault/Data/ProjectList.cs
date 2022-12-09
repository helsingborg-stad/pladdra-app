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
        public List<ProjectReference> projects;
        public ProjectList(List<ProjectReference> projects)
        {
            this.projects = projects;
        }
    }
}