using UnityEngine;

namespace Pladdra.Data
{
    /// <summary>
    /// For testing with a local json.
    /// </summary>
    [System.Serializable]
    public class LocalTestProjectLink : ProjectLink
    {
        public TextAsset json; 
    }
}