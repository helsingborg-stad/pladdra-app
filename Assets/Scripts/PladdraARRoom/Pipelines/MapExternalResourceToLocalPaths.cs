using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dialogs;

namespace Pipelines
{
    public class MapExternalResourceToLocalPaths : TaskYieldInstruction<Dictionary<string, string>>
    {
        public MapExternalResourceToLocalPaths(IWebResourceManager wrm, IEnumerable<string> urls, Action<Dictionary<string, string>> callback) : base(() => wrm.GetResourcePaths(urls), callback)
        {
        }
    }
}
