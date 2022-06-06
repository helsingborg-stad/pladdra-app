using System;
using System.Threading.Tasks;
using Abilities.ARRoomAbility;
using Data.Dialogs;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.EditHistory;

namespace Workspace
{
    public interface IWorkspace: IHasWorkspaceLayers
    {
        string Name { get; } 
        IWorkspaceEditHistoryActions HistoryActions { get; }
        IDialogProjectRepository DialogProjectRepository { get;  }
        IWorkspaceActions Actions { get;  }
        DialogScene GetSceneDescription();
        void UseScene(DialogScene scene);
        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseLayer(string layer);
        void ClearHud();
        void UseUxHandler(IUxHandler handler);
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
    }
}