using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Workspace
{
    public class WorkspaceObjectsManager : IWorkspaceObjectsManager
    {
        public class Item : IWorkspaceObject
        {
            public GameObject GameObject { get; set; }
            public IDictionary<string, GameObject> LayerObjects { get; set; }
            public WorkspaceObject WorkspaceObject { get; set; }
            public IWorkspaceResource WorkspaceResource { get; set; }
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
            var go = Object.Instantiate(itemPrefab, targetParent.transform);
            go.SetActive(false);
            TransformItem(go, position, rotation, scale);

            var layerObjects = resource.LayerPrefabs
                .ToDictionary(
                    kv => kv.Key /* layer name */,
                    kv =>
                    {
                        var cgo = Object.Instantiate(kv.Value, go.transform);
                        cgo.SetActive(false);
                        cgo.transform.SetParent(go.transform, false);
                        return cgo;
                    });
            
            var item = new Item
            {
                GameObject = go,
                WorkspaceObject = go.GetComponent<WorkspaceObject>(),
                WorkspaceResource = resource,
                LayerObjects = layerObjects
            };
            Items.Add(item);
            go.SetActive(true);
            return item;
        }

        public void DestroyItem(GameObject go)
        {
            var item = Items.Find(item => item.GameObject == go);
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