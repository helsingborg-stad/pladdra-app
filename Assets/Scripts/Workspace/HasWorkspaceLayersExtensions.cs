using System.Collections.Generic;
using System.Linq;

namespace Workspace
{
    public static class HasWorkspaceLayersExtensions
    {
        public static T UseLayers<T>(this T hwl, params IWorkspaceLayer[] layers) where T: IHasWorkspaceLayers
        {
            return hwl.UseLayers(layers.Select(l => l.Name).ToArray());
        }

        public static T UseLayers<T>(this T hwl, params string[] layers) where T: IHasWorkspaceLayers
        {
            var s = new HashSet<string>(layers);
            hwl.UseLayers(l => s.Contains(l));
            return hwl;
        }
    }
}