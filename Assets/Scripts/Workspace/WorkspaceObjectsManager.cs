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

        public void SpawnItem(GameObject targetParent, IWorkspaceResource resource, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var go = Object.Instantiate(itemPrefab, targetParent.transform);
            go.SetActive(false);
            TransformItem(go, position, rotation, scale);

            var resourceGo = Object.Instantiate(resource.Prefab, go.transform);
            resourceGo.SetActive(true);
            resourceGo.transform.SetParent(go.transform, false);

            Items.Add(new Item
            {
                GameObject = go,
                WorkspaceObject = go.GetComponent<WorkspaceObject>(),
                WorkspaceResource = resource
            });

            go.SetActive(true);
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
            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.transform.localScale = scale;
        }
    }
}