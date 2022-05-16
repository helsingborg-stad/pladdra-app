using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dialogs;

namespace Pipelines
{
    public class MapExternalResourceToLocalPaths : TaskYieldInstruction<Dictionary<string, string>>
    {
        public MapExternalResourceToLocalPaths(IWebResourceManager wrm, DialogProject project, Action<Dictionary<string, string>> callback): this(
            wrm,
            project.Resources.Select(resource => resource.Url),
            callback
        )
        {
        }

        public MapExternalResourceToLocalPaths(IWebResourceManager wrm, IEnumerable<string> urls, Action<Dictionary<string, string>> callback) : base(() => wrm.GetResourcePaths(urls), callback)
        {
        }
    }
}
