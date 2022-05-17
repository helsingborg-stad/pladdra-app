using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserSelectWorkspaceActions: AbstractUxHandler
    {
        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            workspace.UseHud("user-can-chose-workspace-action-hud", root =>
            {
                root.Q<Button>("edit-plane").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToPositionPlane());
                };
                root.Q<Button>("edit-objects").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToPositionObjects());
                };
                root.Q<Button>("inventory").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToSpawnItemFromResource());
                };
                root.Q<Button>("load-scene").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToLoadWorkspaceScene());
                };
                root.Q<Button>("save-scene").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToSaveWorkspaceScene());
                };
            });
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }
    }
}