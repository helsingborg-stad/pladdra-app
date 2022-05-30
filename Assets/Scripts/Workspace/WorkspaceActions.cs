using System;
using System.Collections.Generic;

namespace Workspace
{
    public class WorkspaceActions : IWorkspaceActions
    {
        private Workspace Workspace { get; }
        private IWorkspaceScene Scene { get; }
        private Dictionary<string, Action<IWorkspaceScene, IWorkspace>> Actions { get; }
        public WorkspaceActions(Workspace workspace, IWorkspaceScene scene)
        {
            Workspace = workspace;
            Scene = scene;
            Actions = new Dictionary<string, Action<IWorkspaceScene, IWorkspace>>();
        }

        public bool HasAction(string name)
        {
            return Actions.ContainsKey(name);
        }

        public void RegisterAction(string name, Action<IWorkspaceScene, IWorkspace> action)
        {
            Actions[name] = action;
        }

        public void DispatchAction(string name)
        {
            Actions[name](Scene, Workspace);
        }
    }
}