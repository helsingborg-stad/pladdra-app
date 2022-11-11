using System.Collections.Generic;

namespace Pladdra.Workspace
{
    public interface IWorkspaceResourceCollection
    {
        IWorkspaceResource TryGetResource(string resourceId);
        IEnumerable<IWorkspaceResource> Resources { get; }
    }
}