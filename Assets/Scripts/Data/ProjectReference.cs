using UnityEngine;

namespace Pladdra.Data
{
    /// <summary>
    /// Reference to a project. Contains name, url to json, description and image.
    /// </summary>
    [System.Serializable]
    public class ProjectReference
    {
        public ProjectReference(string id, string name, string description, string url)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.url = url;
        }
        public string id;
        public string name;
        public string description;
        public string url;
        public TextAsset json;
        public Texture2D image;
    }
}