using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserToLoadWorkspaceScene : AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }

        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);
            var repo = scene.DialogProjectRepository;
            scene.WaitForThen(() => repo.LoadScenes(), scenes => UseUserCanSelectSceneHud(scene, scenes));
        }

        private void UseUserCanSelectSceneHud(IWorkspaceScene scene, Dictionary<string, DialogScene> scenes)
        {
            scene.UseHud("user-can-select-scene-hud", root =>
            {
                var container = root.Q<VisualElement>("content");
                
                var listItem = Resources.Load<VisualTreeAsset>("saved-scene-item");
                foreach (var kv in scenes)
                {
                    var itemInstance = listItem.Instantiate();
                    itemInstance.Q<Label>("card-title").text = kv.Key;
                    itemInstance.Q<Button>("card").clicked += () =>
                    {
                        /*
                        scene.ObjectsManager.SpawnItem(scene.Plane, item, Vector3.zero, new Quaternion(),
                            new Vector3(1, 1, 1));
                        */
                        scene.UseScene(kv.Key, kv.Value);
                        scene.UseUxHandler(new AllowUserSelectWorkspaceActions());
                    };
                    container.Add(itemInstance);
                }
                root.Q<Button>("close").clicked += () => { scene.UseUxHandler(new AllowUserSelectWorkspaceActions()); };
            });
        }

    }
}