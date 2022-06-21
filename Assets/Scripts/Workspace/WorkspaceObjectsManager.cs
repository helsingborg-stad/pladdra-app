using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Workspace
{
    public class WorkspaceObjectsManager : IWorkspaceObjectsManager
    {
        private class Item : IWorkspaceObject
        {
            public GameObject GameObject { get; set; }
            public IDictionary<string, GameObject> LayerObjects { get; set; }
            public WorkspaceObject WorkspaceObject { get; set; }
            public IWorkspaceResource WorkspaceResource { get; set; }
            public bool ContainsGameObject(GameObject go)
            {
                return (go == GameObject) || (LayerObjects?.Values.Contains(go) == true);
            }

            public void UseLayers(Func<string, bool> layerShouldBeUsed)
            {
                foreach (var kv in LayerObjects)
                {
                    kv.Value.SetActive(layerShouldBeUsed(kv.Key));
                }
            }
        }
        private readonly GameObject itemPrefab;
        private List<Item> Items { get; }
        public IEnumerable<IWorkspaceObject> Objects => Items;
        public WorkspaceObjectsManager(GameObject itemPrefab)
        {
            this.itemPrefab = itemPrefab;
            Items = new List<Item>();
        }

        public IWorkspaceObject SpawnItem(IWorkspaceResource resource, GameObject targetParent, Vector3 position,
            Quaternion rotation, Vector3 scale)
        {
            var itemPlaceholder = CreateChildGameObject(itemPrefab, targetParent, (child, parent) => TransformItem(child, position, rotation, scale));

            var layerObjects = (
                    from kv in resource.LayerPrefabs
                    let layer = kv.Key
                    let layerPrefab = kv.Value
                    let layerPlaceHolder = CreateChildGameObject(itemPrefab, itemPlaceholder)
                    let layerObject = CreateChildGameObject(layerPrefab, layerPlaceHolder)
                    select new { layer, layerPlaceHolder })
                .ToDictionary(o => o.layer, o => o.layerPlaceHolder);
            
            // layerObjects["model"].transform.SetParent(layerObjects["marker"].transform, false);
            var item = new Item
            {
                GameObject = itemPlaceholder,
                WorkspaceObject = itemPlaceholder.GetComponent<WorkspaceObject>(),
                WorkspaceResource = resource,
                LayerObjects = layerObjects
            };
            Items.Add(item);
            itemPlaceholder.SetActive(true);
            return item;

            GameObject CreateChildGameObject(GameObject prefab, GameObject parent, Action<GameObject, GameObject> init = null)
            {
                var go = Object.Instantiate(prefab, parent.transform);
                go.transform.SetParent(parent.transform, false);
                go.SetActive(true);
                init?.Invoke(go, parent);
                return go;
            }
                
        }

        public void DestroyItem(GameObject go)
        {
            var item = Items.Find(item => item.ContainsGameObject(go));
            Items.Remove(item);
            UnityEngine.Object.Destroy(go);
        }

        public void DestroyAll()
        {
            foreach (var item in Items)
            {
                UnityEngine.Object.Destroy(item.GameObject);
            }
            Items.Clear();
        }
        
        public void UseLayers(Func<string, bool> layerShouldBeUsed)
        {
            foreach (var item in Items)
            {
                item.UseLayers(layerShouldBeUsed);
            }
        }

        private void TransformItem(GameObject go, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            go.transform.localPosition = new Vector3(position.x, position.y, position.z);
            go.transform.localRotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            go.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }
}