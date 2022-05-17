using System;
using System.Threading.Tasks;
using Data.Dialogs;
using Repository;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace
{
    public interface IWorkspaceScene
    {
        IDialogProjectRepository DialogProjectRepository { get;  }
        GameObject Plane { get; }
        IWorkspaceObjectsManager ObjectsManager { get; }
        IWorkspaceResourceCollection Resources { get; }

        void UseScene(string name, DialogScene scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseUxHandler(IUxHandler handler);
        DialogScene CreateWorkspaceSceneDescription();
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
    }
}