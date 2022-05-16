using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObject
    {
        GameObject GameObject { get; }
        public IWorkspaceResource WorkspaceResource { get; }
    }
}