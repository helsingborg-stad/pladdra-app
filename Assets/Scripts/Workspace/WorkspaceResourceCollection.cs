using System.Collections.Generic;
using System.Linq;

namespace Workspace
{
    public class WorkspaceResourceCollection : IWorkspaceResourceCollection
    {
        public IWorkspaceResource TryGetResource(string resourceId) =>
            Resources.FirstOrDefault(r => r.ResourceID == resourceId);

        public IEnumerable<IWorkspaceResource> Resources { get; set; }
    }
}