using System.Linq;
using Data.Dialogs;
using UnityEngine;

namespace Workspace
{
    public class WorkspaceScene: IWorkspaceScene
    {
        public string Name { get; private set; }
        public GameObject Plane { get; private set;  }
        public IWorkspaceObjectsManager ObjectsManager { get; private set;  }
        public IWorkspaceResourceCollection Resources { get; private set;  }
        public DialogScene CreateWorkspaceSceneDescription(string name) => DialogScene.Describe(this, name);
        public IWorkspaceObject SpawnItem(IWorkspaceResource item)
        {
            return
                ObjectsManager
                    .SpawnItem(item, Plane, Vector3.zero, new Quaternion(), new Vector3(1, 1, 1))
                    .UseLayers("marker");
        }

        public void UseScene(DialogScene scene)
        {
            ObjectsManager.DestroyAll();

            UpdateTransform(Plane, scene?.Plane);

            foreach (var item in scene?.Items ??
                                 Enumerable.Empty<DialogScene.ItemDescription>())
            {
                var resource = Resources.TryGetResource(item.ResourceId);
                if (resource != null)
                {
                    SpawnItem(resource);
                    /*
                    ObjectsManager.SpawnItem(resource, Scene.Plane, item.Position?.ToVector3() ?? new Vector3(1, 1, 1),
                        item.Rotation?.ToQuaternion() ?? new Quaternion(),
                        item.Scale?.ToVector3() ?? new Vector3(1, 1, 1));
                    */
                }
            }

            void UpdateTransform(GameObject go, DialogScene.TransformDescription t)
            {
                go.transform.localPosition = t?.Position?.ToVector3() ?? go.transform.localPosition;
                go.transform.localScale = t?.Scale?.ToVector3() ?? go.transform.localScale;
                go.transform.localRotation = t?.Rotation?.ToQuaternion() ?? go.transform.localRotation;
            }

            Name = scene?.Name ?? "";
        }

        public WorkspaceScene(GameObject plane, IWorkspaceObjectsManager objectsManager, IWorkspaceResourceCollection resources)
        {
            Plane = plane;
            ObjectsManager = objectsManager;
            Resources = resources;
        }
    }
}