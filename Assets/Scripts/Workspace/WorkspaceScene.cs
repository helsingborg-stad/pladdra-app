using Data.Dialogs;
using UnityEngine;

namespace Workspace
{
    public class WorkspaceScene: IWorkspaceScene
    {
        public GameObject Plane { get; private set;  }
        public IWorkspaceObjectsManager ObjectsManager { get; private set;  }
        public IWorkspaceResourceCollection Resources { get; private set;  }
        public DialogScene CreateWorkspaceSceneDescription(string name) => DialogScene.Describe(this, name);

        public WorkspaceScene(GameObject plane, IWorkspaceObjectsManager objectsManager, IWorkspaceResourceCollection resources)
        {
            Plane = plane;
            ObjectsManager = objectsManager;
            Resources = resources;
        }
    }
}