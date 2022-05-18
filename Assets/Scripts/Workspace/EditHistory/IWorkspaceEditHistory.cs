using System;
using Data.Dialogs;

namespace Workspace.EditHistory
{
    public interface IWorkspaceEditHistory
    {
        bool CanUndo();
        bool CanRedo();
        void SaveSnapshot(IWorkspace workspace);
         void Undo(Action<DialogScene> restore);
         void Redo(Action<DialogScene> restore);
    }
}