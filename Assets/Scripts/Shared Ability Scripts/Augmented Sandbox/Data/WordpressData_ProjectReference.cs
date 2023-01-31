using UnityEngine;

namespace Pladdra.Data
{
    /// <summary>
    /// Reference to a project. Contains name, url to json, description and image.
    /// </summary>
    [System.Serializable]
    public class WordpressData_ProjectReference : WordpressData
    {
        public ProjectReference_Acf acf;

        public ProjectReference MakeProjectReference(string projectBaseUrl)
        {
            string url = string.Format(projectBaseUrl, id);
            return new ProjectReference(id, title.rendered, acf.settings.description, url);
        }
    }

    [System.Serializable]
    public class ProjectReference_Acf
    {
        public ProjectReference_AcfSettings settings;
    }

    [System.Serializable]
    public class ProjectReference_AcfSettings
    {
        public string description;
    }
}