using System.Collections.Generic;
using Lean.Common;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace
{
    public class AllowUserToPositionPlane : AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return new[] { scene.Plane };
        }

        protected override void OnSelected(IWorkspaceScene scene, GameObject go)
        {
            scene.UseHud("user-has-selected-plane-hud", root =>
            {
                root.Q<Button>("done").clicked += () =>
                {
                    DeselectAll();
                    scene.UseUxHandler(new AllowUserToPositionObjects());
                };
            });
        }

        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);
            SelectObject(scene.Plane);
        }
    }
}