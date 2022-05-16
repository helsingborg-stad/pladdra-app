using UnityEngine;

namespace Workspace
{
    public class WorkspaceResource : IWorkspaceResource
    {
        public string ResourceID { get; set; }
        public GameObject Prefab { get; set; }
    }
}

