using UnityEngine;

namespace Pladdra.Workspace
{
    public interface IWorkspaceMarker
    {
        Texture2D Image { get;}
        float Width { get;}
        float Height{ get;}
    }
}