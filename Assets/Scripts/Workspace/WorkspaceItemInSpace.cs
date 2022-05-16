using UnityEngine;

namespace Workspace
{
    public class WorkspaceItemInSpace : IWorkspaceItemInSpace
    {
        public string ResourceId { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Scale { get; set; }

        public Quaternion Rotation { get; set; }
    }
}