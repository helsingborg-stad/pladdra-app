namespace Pladdra.Workspace.EditHistory
{
    public interface IWorkspaceEditHistoryActions
    {
        bool CanUndo { get; }
        bool CanRedo { get; }
        void Undo();
        void Redo();
    }
}