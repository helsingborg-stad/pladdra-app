using System.Collections.Generic;

namespace Workspace
{
    public class WorkspaceCosmos : IWorkspaceCosmos
    {
        public IEnumerable<IWorkspaceItemInSpace> SpaceItems { get; set; }
    }
}