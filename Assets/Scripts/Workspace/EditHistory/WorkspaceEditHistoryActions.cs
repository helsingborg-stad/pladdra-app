using System;
using Data.Dialogs;
using Pladdra.Data;

namespace Pladdra.Workspace.EditHistory
{
    public class WorkspaceEditHistoryActions : IWorkspaceEditHistoryActions
    {
        private IWorkspaceEditHistory WorkspaceEditHistory { get; }
        private Action<UserProposal> Restore { get; }

        public WorkspaceEditHistoryActions(IWorkspaceEditHistory workspaceEditHistory, Action<UserProposal> restore)
        {
            WorkspaceEditHistory = workspaceEditHistory;
            Restore = restore;
        }

        public bool CanUndo => WorkspaceEditHistory.CanUndo();
        public bool CanRedo => WorkspaceEditHistory.CanRedo();

        public void Undo()
        {
            WorkspaceEditHistory.Undo(Restore);
        }

        public void Redo()
        {
            WorkspaceEditHistory.Redo(Restore);
        }
    }
}