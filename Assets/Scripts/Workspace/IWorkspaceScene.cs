using System.Collections.Generic;
using Data.Dialogs;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceScene
    {
        string Name { get; }
        GameObject Plane { get; }
        IWorkspaceObjectsManager ObjectsManager { get; }
        IWorkspaceResourceCollection Resources { get; }
        DialogScene CreateWorkspaceSceneDescription(string name);
        IWorkspaceObject SpawnItem(IWorkspaceResource item);
        IEnumerable<IWorkspaceLayer> Layers { get; }
        void UseScene(DialogScene scene);
    }
}