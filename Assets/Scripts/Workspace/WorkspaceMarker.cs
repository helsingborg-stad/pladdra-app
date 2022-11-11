using UnityEngine;

namespace Pladdra.Workspace
{
    public class WorkspaceMarker : IWorkspaceMarker
    {
        public Texture2D Image { get; set; }
        public float Width { get; set;  }
        public float Height { get; set;  }
    }
}