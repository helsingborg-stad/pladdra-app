using UnityEngine;
using Workspace;

namespace UXHandlers
{
    public interface IUxHandlerEvents
    {
        void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go);
        void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go);
    }
}