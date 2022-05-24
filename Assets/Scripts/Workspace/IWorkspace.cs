using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities.ARRoomAbility;
using Data.Dialogs;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.EditHistory;
using Workspace.UxHandlers;

namespace Workspace
{
    public interface IWorkspace
    {
        string Name { get; } 
        IWorkspaceEditHistoryActions HistoryActions { get; }
        IDialogProjectRepository DialogProjectRepository { get;  }
        IWorkspaceActions Actions { get;  }
        DialogScene GetSceneDescription();
        void UseScene(DialogScene scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void ClearHud();
        void UseUxHandler(IUxHandler handler);
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
        
    }
}