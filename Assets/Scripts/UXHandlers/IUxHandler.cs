using Workspace;

namespace UXHandlers
{
    public interface IUxHandler
    {
        void Activate(IWorkspaceScene scene);
        void Deactivate(IWorkspaceScene scene);
    }
}