using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Data
{
    [System.Serializable]
    public class LocalTestProjectList
    {
        public List<LocalTestProjectLink> projects;
        public LocalTestProjectList(List<LocalTestProjectLink> projects)
        {
            this.projects = projects;
        }
    }
}