using System;
using Pladdra.Data;

namespace Pladdra.Workspace.EditHistory
{
    public interface IWorkspaceEditHistory
    {
        bool CanUndo();
        bool CanRedo();
        void SaveSnapshot(IWorkspace workspace);
         void Undo(Action<UserProposal> restore);
         void Redo(Action<UserProposal> restore);
    }
}