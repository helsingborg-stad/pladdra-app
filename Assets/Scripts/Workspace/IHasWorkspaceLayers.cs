using System;

namespace Workspace
{
    public interface IHasWorkspaceLayers
    {
        void UseLayers(Func<string, bool> layerShouldBeUsed);
    }
}