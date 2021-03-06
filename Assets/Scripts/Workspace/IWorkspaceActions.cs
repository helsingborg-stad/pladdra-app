using System;

namespace Workspace
{
    public interface IWorkspaceActions
    {
        bool HasAction(string name);
        void RegisterAction(string name, Action<IWorkspaceScene, IWorkspace> action);
        void DispatchAction(string name);
    }
}