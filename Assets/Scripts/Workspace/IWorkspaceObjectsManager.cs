using System;
using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObjectsManager: IHasWorkspaceLayers
    {
        IEnumerable<IWorkspaceObject> Objects { get; }

        IWorkspaceObject SpawnItem(IWorkspaceResource resource, GameObject targetParent, Vector3 position,
            Quaternion rotation, Vector3 scale);
        void DestroyItem(GameObject go);
        void DestroyAll();
    }
}