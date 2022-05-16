using System.Collections.Generic;

namespace Workspace
{
    public interface IWorkspaceCosmos
    {
        IEnumerable<IWorkspaceItemInSpace> SpaceItems { get; }
    }
}