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
                    itemInstance.Q<IMGUIContainer>().style.backgroundImage = item.LayerThumbnails[scene.Layers.First().Name];
                    itemInstance.Q<Label>().text = item.ResourceID;
                    itemInstance.Q<Button>().clicked += () =>
                    {
                        scene.SpawnItem(item);
                        workspace.Actions.DispatchAction("default");
                    };
                    container.Add(itemInstance);
                });


                root.Q<Button>("close").clicked += () => { workspace.Actions.DispatchAction("default"); };
            });
        }
    }
}