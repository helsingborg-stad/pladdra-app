using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObjectsManager
    {
        IEnumerable<IWorkspaceObject> Objects { get; }

        void SpawnItem(GameObject targetParent, IWorkspaceResource resource, Vector3 position, Quaternion rotation, Vector3 scale);
        void DestroyItem(GameObject go);
    }
}