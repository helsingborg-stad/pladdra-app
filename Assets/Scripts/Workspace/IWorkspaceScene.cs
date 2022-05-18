using Data.Dialogs;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceScene
    {
        GameObject Plane { get; }
        IWorkspaceObjectsManager ObjectsManager { get; }
        IWorkspaceResourceCollection Resources { get; }
        DialogScene CreateWorkspaceSceneDescription(string name);
/*
        void UseScene(string name, DialogScene scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseUxHandler(IUxHandler handler);
        DialogScene CreateWorkspaceSceneDescription();
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
*/
    }
}