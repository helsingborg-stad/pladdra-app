using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceItemInSpace
    {
        public string ResourceId { get; }
        public Vector3 Position { get; }
        public Vector3 Scale { get; }
        public Quaternion Rotation { get; }
    }
}