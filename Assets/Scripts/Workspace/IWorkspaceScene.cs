using System;
using Data.Dialogs;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.Snapshot;

namespace Workspace
{
    public interface IWorkspaceScene
    {
        IDialogProjectRepository DialogProjectRepository { get;  }
        GameObject Plane { get; }
        IWorkspaceObjectsManager ObjectsManager { get; }
        IWorkspaceResourceCollection Resources { get; }
        
        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseUxHandler(IUxHandler handler);
        WorkspaceSceneDescription CreateWorkspaceSceneDescription();
    }
}