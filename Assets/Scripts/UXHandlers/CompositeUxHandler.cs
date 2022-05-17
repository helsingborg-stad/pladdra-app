using Workspace;

namespace UXHandlers
{
    public class CompositeUxHandler : IUxHandler
    {
        public CompositeUxHandler(params IUxHandler[] handlers)
        {
            this.Handlers = handlers;
        }

        private IUxHandler[] Handlers { get; set; }

        public void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var handler in Handlers)
            {
                handler.Activate(scene, workspace);
            }
        }

        public void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var handler in Handlers)
            {
                handler.Deactivate(scene, workspace);
            }
        }
    }
}