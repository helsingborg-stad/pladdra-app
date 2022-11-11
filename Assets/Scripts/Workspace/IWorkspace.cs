using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities.ARRoomAbility;
using Data.Dialogs;
using UnityEngine.UIElements;
using UXHandlers;
using Pladdra.Workspace.EditHistory;
using Pladdra.Workspace.UxHandlers;
using Pladdra.Data;

namespace Pladdra.Workspace
{
    public interface IWorkspace
    {
        string Name { get; } 
        IWorkspaceEditHistoryActions HistoryActions { get; }
        IDialogProjectRepository DialogProjectRepository { get;  }
        IWorkspaceActions Actions { get;  }
        UserProposal GetSceneDescription();
        void UseScene(UserProposal scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void ClearHud();
        void UseUxHandler(IUxHandler handler);
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
        
    }
}