using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceResource
    {
        string ResourceID { get; }
        GameObject Prefab { get; }
    }
}