using UnityEngine;

namespace Workspace.UxHandlers.ObjectInspectors
{
    public interface IGameObjectPositionInspector
    {
        void OnPositionChanged(Vector3 position);
    }
}