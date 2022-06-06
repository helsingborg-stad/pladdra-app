using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{

    public interface IWorkspaceResource
    {
        string ResourceID { get; }
        Dictionary<string, GameObject> LayerPrefabs { get; set; }
    }
}