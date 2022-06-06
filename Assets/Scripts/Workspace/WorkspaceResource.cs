using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public class WorkspaceResource : IWorkspaceResource
    {
        public string ResourceID { get; set; }
        public Dictionary<string, GameObject> LayerPrefabs { get; set; }
    }
}

