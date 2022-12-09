using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    [System.Serializable]
    public class LocalTestProjectList
    {
        public List<LocalTestProjectReference> projects;
        public LocalTestProjectList(List<LocalTestProjectReference> projects)
        {
            this.projects = projects;
        }
    }
}