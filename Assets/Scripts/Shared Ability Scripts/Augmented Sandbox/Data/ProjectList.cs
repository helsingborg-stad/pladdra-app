using System;
using System.Collections.Generic;
using Pladdra.Data;

namespace Pladdra.Data
{
    /// <summary>
    /// Helper class to load only relevant data from the project info from Wordpress
    /// </summary>
    public class ProjectList : WordpressData
    {
        public WordpressData_ProjectReference[] projects;

        internal List<ProjectReference> MakeProjectList(string projectBaseUrl)
        {
            List<ProjectReference> projectReferences = new List<ProjectReference>();

            foreach (WordpressData_ProjectReference project in projects)
            {
                string url = string.Format(projectBaseUrl, project.id);
                projectReferences.Add(new ProjectReference(project.id, project.title.rendered, project.acf.settings.description, url));
            }
            return projectReferences;
        }
    }
}