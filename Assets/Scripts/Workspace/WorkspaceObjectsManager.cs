using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Workspace
{
    public class WorkspaceObjectsManager : IWorkspaceObjectsManager
    {
        public class Item : IWorkspaceObject
        {
            public GameObject GameObject { get; set; }
            public IEnumerable<GameObject> ChildGameObjects { get; set;  }
            public WorkspaceObject WorkspaceObject { get; set; }
            public IWorkspaceResource WorkspaceResource { get; set; }
        }
        private readonly GameObject itemPrefab;
        private List<Item> Items { get; }
        public IEnumerable<IWorkspaceObject> Objects => Items;
        public WorkspaceObjectsManager(GameObject itemPrefab)
        {
            this.itemPrefab = itemPrefab;
            Items = new List<Item>();
        }

        public GameObject SpawnItem(IWorkspaceResource resource, GameObject targetParent, Vector3 position,
            Quaternion rotation, Vector3 scale)
        {
            var go = Object.Instantiate(itemPrefab, targetParent.transform);
            go.SetActive(false);
            TransformItem(go, position, rotation, scale);

            var children = resource.Prefabs.Select((prefab, index) =>
            {
                var cgo = Object.Instantiate(prefab, go.transform);
                cgo.SetActive(index == 0);
                cgo.transform.SetParent(go.transform, false);
                return cgo;
            }).ToList();


            Items.Add(new Item
            {
                GameObject = go,
                WorkspaceObject = go.GetComponent<WorkspaceObject>(),
                WorkspaceResource = resource,
                ChildGameObjects = children
            });

            go.SetActive(true);
            return go;
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

        private void TransformItem(GameObject go, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            go.transform.localPosition = new Vector3(position.x, position.y, position.z);
            go.transform.localRotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            go.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }
}