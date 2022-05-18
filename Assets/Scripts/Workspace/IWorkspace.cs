using System;
using System.Threading.Tasks;
using Data.Dialogs;
using Repository;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace
{
    public interface IWorkspace
    {
        string Name { get; }
        IDialogProjectRepository DialogProjectRepository { get;  }
        void UseScene(DialogScene scene);

        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void UseUxHandler(IUxHandler handler);
        void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then);
    }
}