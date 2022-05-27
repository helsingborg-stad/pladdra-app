using System;
using UnityEngine;
using Workspace;

namespace UXHandlers
{
    public interface IUxHandlerTraitContext
    {
        GameObject GameObject { get; }
        IWorkspace Workspace { get; }
        IWorkspaceScene Scene { get; }
        IUxHandlerEvents Events { get; }
        TComponent TryConfigureComponent<TComponent>(Action<TComponent> configure);
    }
}