using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace
{
    public class AllowUserToSpawnItemFromResource : AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }

        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);

            scene.UseHud("user-can-chose-resource-to-spawn-hud", root =>
            {
                var container = root.Q<VisualElement>("content");
                var items = scene.Resources.Resources.ToList();
                var listItem = Resources.Load<VisualTreeAsset>("resource-item");
                items.ForEach(item =>
                {
                    var itemInstance = listItem.Instantiate();
                    itemInstance.Q<Label>().text = item.ResourceID;
                    itemInstance.Q<Button>().clicked += () =>
                    {
                        scene.ObjectsManager.SpawnItem(scene.Plane, item, Vector3.zero, new Quaternion(),
                            new Vector3(1, 1, 1));
                        scene.UseUxHandler(new AllowUserToPositionObjects());
                    };
                    container.Add(itemInstance);
                });


                root.Q<Button>("close").clicked += () => { scene.UseUxHandler(new AllowUserToPositionObjects()); };
            });
        }
    }
}