using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserToSpawnItemFromResource : AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);

            workspace.UseHud("user-can-chose-resource-to-spawn-hud", root =>
            {
                var container = root.Q<VisualElement>("content");
                var resources = scene.Resources.Resources.ToList();
                var listItem = Resources.Load<VisualTreeAsset>("resource-item");
                resources.ForEach(item =>
                {
                    var itemInstance = listItem.Instantiate();
                    itemInstance.Q<Label>().text = item.ResourceID;
                    itemInstance.Q<Button>().clicked += () =>
                    {
                        scene.ObjectsManager.SpawnItem(item, scene.Plane, Vector3.zero, new Quaternion(), new Vector3(1, 1, 1));
                        workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
                    };
                    container.Add(itemInstance);
                });


                root.Q<Button>("close").clicked += () => { workspace.UseUxHandler(new AllowUserSelectWorkspaceActions()); };
            });
        }
    }
}