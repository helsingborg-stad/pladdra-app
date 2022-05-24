using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObjectsManager
    {
        IEnumerable<IWorkspaceObject> Objects { get; }

        GameObject SpawnItem(IWorkspaceResource resource, GameObject targetParent, Vector3 position,
            Quaternion rotation, Vector3 scale);
        void DestroyItem(GameObject go);
        void DestroyAll();
    }
}