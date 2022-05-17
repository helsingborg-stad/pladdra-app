using Workspace;

namespace UXHandlers
{
    public interface IUxHandler
    {
        void Activate(IWorkspaceScene scene, IWorkspace workspace);
        void Deactivate(IWorkspaceScene scene, IWorkspace workspace);
    }
}