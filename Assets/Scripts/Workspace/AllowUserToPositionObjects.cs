using System.Collections.Generic;
using System.Linq;
using Lean.Common;
using Piglet;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using UXHandlers;
using Workspace.UxHandlers;
using Workspace.UxHandlers.ObjectInspectors;

namespace Workspace
{
    public class AllowUserToPositionObjects: AbstractUxHandler
    {
        private IGameObjectPositionInspector GameObjectPositionInspector { get; set; }

        public AllowUserToPositionObjects()
        {
            GameObjectPositionInspector = new NullGameObjectPositionInspector();
        }


        protected override void OnSelected(IWorkspaceScene scene, GameObject go)
        {
            // add component that listens for position changes
            var tc = go.GetOrAddComponent<TransformChangedHandler>();
            tc.enabled = true;
            tc.OnPositionChanged.AddListener(position => GameObjectPositionInspector.OnPositionChanged(position));

            UseUserHasSelectedWorkspaceObjectHud(scene, go);
        }
        
        protected override void OnDeselected(IWorkspaceScene scene, GameObject go)
        {
            go.RemoveComponent<TransformChangedHandler>();
            scene.UseUxHandler(new AllowUserToPositionObjects());
        }

        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);
            UseUserCanChoseWorkspaceActionHud(scene);
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return scene.ObjectsManager.Objects.Select(o => o.GameObject);
        }

        protected void UseUserCanChoseWorkspaceActionHud(IWorkspaceScene scene)
        {
            scene.UseHud("user-can-chose-workspace-action-hud", root =>
            {
                root.Q<Button>("edit-plane").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToPositionPlane());
                };
                root.Q<Button>("inventory").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToSpawnItemFromResource());
                };
                root.Q<Button>("save").clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToSaveWorkspaceScene());
                };
            });
        }
        protected void UseUserHasSelectedWorkspaceObjectHud(IWorkspaceScene scene, GameObject go) {
            // Create our nice HUD
            scene.UseHud("user-has-selected-workspace-object-hud", root =>
            {
                root.Q<Button>("remove").clicked += () =>
                {
                    scene.ObjectsManager.DestroyItem(go);
                    scene.UseUxHandler(new AllowUserToPositionObjects());
                };
                root.Q<Button>("done").clicked += () =>
                {
                    go.GetComponent<LeanSelectable>().Deselect();
                };

                // and attach it to our position changed logic 
                GameObjectPositionInspector = new GameObjectPositionInspector(go, root);
            });
        }
    }
}