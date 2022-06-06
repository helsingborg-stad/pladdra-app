using System.Collections.Generic;
using System.Linq;

namespace Workspace
{
    public static class HasWorkspaceLayersExtensions
    {
        public static void UseLayers(this IHasWorkspaceLayers hwl, params IWorkspaceLayer[] layers)
        {
            hwl.UseLayers(layers.Select(l => l.Name).ToArray());
        }

        public static void UseLayers(this IHasWorkspaceLayers hwl, params string[] layers)
        {
            var s = new HashSet<string>(layers);
            hwl.UseLayers(l => s.Contains(l));
        }
    }
}