using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserToPositionPlane : AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return new[] { scene.Plane };
        }

        protected override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnSelected(scene, workspace, go);

            workspace.UseHud("user-has-selected-plane-hud", root =>
            {
                root.Q<Button>("done").clicked += () =>
                {
                    DeselectAll();
                    workspace.Actions.DispatchAction("default");
                };
            });
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            SelectObject(scene.Plane);
        }
    }
}