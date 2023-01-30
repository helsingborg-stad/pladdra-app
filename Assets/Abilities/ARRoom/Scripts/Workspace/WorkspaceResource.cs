using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Workspace
{
    public class WorkspaceResource : IWorkspaceResource
    {
        public string ResourceID { get; set; }
        public GameObject Prefab { get; set; }
        public IEnumerable<GameObject> Prefabs { get; set; }
        
        public IEnumerable<Texture2D> Thumbnails { get; set; }
    }
}

