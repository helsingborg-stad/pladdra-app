using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserSelectWorkspaceActions: AbstractUxHandler
    {
        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);
            scene.UseHud("user-can-chose-workspace-action-hud", root =>
            {
                root.Q<Button>("edit-plane").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToPositionPlane());
                };
                root.Q<Button>("edit-objects").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToPositionObjects());
                };
                root.Q<Button>("inventory").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToSpawnItemFromResource());
                };
                root.Q<Button>("load-scene").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToLoadWorkspaceScene());
                };
                root.Q<Button>("save-scene").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToSaveWorkspaceScene());
                };
            });
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }
    }
}