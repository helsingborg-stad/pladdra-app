using UnityEngine;
using Pladdra.Workspace;

namespace UXHandlers
{
    public interface IUxHandlerEvents
    {
        void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go);
        void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go);
    }
}