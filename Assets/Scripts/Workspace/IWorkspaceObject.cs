using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObject
    {
        GameObject GameObject { get; }
        IEnumerable<GameObject> ChildGameObjects { get; }
        
        public IWorkspaceResource WorkspaceResource { get; }
    }
}