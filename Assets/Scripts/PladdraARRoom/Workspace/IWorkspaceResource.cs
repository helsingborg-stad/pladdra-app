using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Workspace
{

    public interface IWorkspaceResource
    {
        string ResourceID { get; }
        IEnumerable<GameObject> Prefabs { get;  }
        IEnumerable<Texture2D> Thumbnails { get;  }
    }
}