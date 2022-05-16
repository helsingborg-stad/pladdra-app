using System.Collections.Generic;

namespace Workspace
{
    public interface IWorkspaceResourceCollection
    {
        IWorkspaceResource TryGetResource(string resourceId);
        IEnumerable<IWorkspaceResource> Resources { get; }
    }
}