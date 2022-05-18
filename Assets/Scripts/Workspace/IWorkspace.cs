using System;
using System.Threading.Tasks;
using Data.Dialogs;
using Repository;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.EditHistory;

namespace Workspace
{
    public interface IWorkspace
    {
        string Name { get; }
        IWorkspaceEditHistoryActions HistoryActions { get; }
        IDialogProjectRepository DialogProjectRepository { get;  }
        DialogScene GetSceneDescription();
        void UseScene(DialogScene scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseUxHandler(IUxHandler handler);
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
    }
}