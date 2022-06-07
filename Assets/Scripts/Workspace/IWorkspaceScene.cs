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
        void UseScene(DialogScene scene);
    }
}