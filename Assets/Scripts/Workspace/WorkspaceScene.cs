using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using UnityEngine;
using Utility;

namespace Workspace
{
    public class WorkspaceScene: IWorkspaceScene
    {
        public string Name { get; private set; }
        public GameObject Plane { get; private set;  }
        public IWorkspaceObjectsManager ObjectsManager { get; private set;  }
        public IWorkspaceResourceCollection Resources { get; private set;  }
        public IEnumerable<IWorkspaceLayer> Layers { get; }
        public DialogScene CreateWorkspaceSceneDescription(string name) => DialogScene.Describe(this, name);
        public IWorkspaceObject SpawnItem(IWorkspaceResource item)
        {
            return
                ObjectsManager
                    .SpawnItem(item, Plane, Vector3.zero, new Quaternion(), new Vector3(1, 1, 1))
                    .UseLayers(Layers.Select(l => l.Name).FirstOrDefault());
        }

        public void UseScene(DialogScene scene)
        {
            ObjectsManager.DestroyAll();

            UpdateTransform(Plane, scene?.Plane);

            var transforms = (
                    from sceneItem in scene?.Items ?? Enumerable.Empty<DialogScene.ItemDescription>()
                    let resource = Resources.TryGetResource(sceneItem.ResourceId)
                    where resource != null
                    let item = SpawnItem(resource)
                    from kv in item.LayerObjects
                    let layer = kv.Key
                    let layerObject = kv.Value
                    let layerTransform = sceneItem?.Layers?.TryGet(layer)
                    where layerTransform != null
                    select UpdateTransform(layerObject, layerTransform))
                .ToArray();

            bool UpdateTransform(GameObject go, DialogScene.TransformDescription t)
            {
                go.transform.localPosition = t?.Position?.ToVector3() ?? go.transform.localPosition;
                go.transform.localScale = t?.Scale?.ToVector3() ?? go.transform.localScale;
                go.transform.localRotation = t?.Rotation?.ToQuaternion() ?? go.transform.localRotation;
                return t != null;
            }

            Name = scene?.Name ?? "";
        }

        public WorkspaceScene(GameObject plane, IWorkspaceObjectsManager objectsManager,
            IWorkspaceResourceCollection resources, IEnumerable<IWorkspaceLayer> layers)
        {
            Plane = plane;
            ObjectsManager = objectsManager;
            Resources = resources;
            Layers = (layers ?? Enumerable.Empty<IWorkspaceLayer>()).ToList();
        }
    }
}