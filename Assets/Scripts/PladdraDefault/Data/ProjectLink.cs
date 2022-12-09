using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    /// <summary>
    /// Reference to a project. Contains name, url to json, description and image.
    /// </summary>
    [System.Serializable]
    public class ProjectReference
    {
        public string name;
        public string url;
        public string description;
        public Texture2D image;
    }
}