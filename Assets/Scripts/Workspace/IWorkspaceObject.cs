using System.Collections.Generic;
using UnityEngine;

namespace Workspace
{
    public interface IWorkspaceObject: IHasWorkspaceLayers
    {
        GameObject GameObject { get; }
        
        IDictionary<string, GameObject> LayerObjects { get; }
        
        public IWorkspaceResource WorkspaceResource { get; }
        bool ContainsGameObject(GameObject go);
    }
}