using System.Collections.Generic;
using System.Linq;
using Lean.Common;
using Piglet;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using UXHandlers;
using Workspace.UxHandlers.ObjectInspectors;

namespace Workspace.UxHandlers
{
    public class AllowUserToPositionObjects: AbstractUxHandler
    {
        private IGameObjectPositionInspector GameObjectPositionInspector { get; set; }

        public AllowUserToPositionObjects()
        {
            GameObjectPositionInspector = new NullGameObjectPositionInspector();
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            UserCanSelectObjectHud(workspace);
        }

        protected override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            // add component that listens for position changes
            var tc = go.GetOrAddComponent<TransformChangedHandler>();
            tc.enabled = true;
            tc.OnPositionChanged.AddListener(position => GameObjectPositionInspector.OnPositionChanged(position));

            UseUserHasSelectedWorkspaceObjectHud(scene, workspace, go);
        }
        
        protected override void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            go.RemoveComponent<TransformChangedHandler>();
            //scene.UseUxHandler(new AllowUserSelectWorkspaceActions());
            UserCanSelectObjectHud(workspace);
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return scene.ObjectsManager.Objects.Select(o => o.GameObject);
        }
        
        private void UseUserHasSelectedWorkspaceObjectHud(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            workspace.UseHud("user-has-selected-workspace-object-hud", root =>
            {
                root.Q<Button>("remove").clicked += () =>
                {
                    scene.ObjectsManager.DestroyItem(go);
                    // scene.UseUxHandler(new AllowUserSelectWorkspaceActions());
                };
                root.Q<Button>("done").clicked += () =>
                {
                    go.GetComponent<LeanSelectable>().Deselect();
                };

                // and attach it to our position changed logic 
                GameObjectPositionInspector = new GameObjectPositionInspector(go, root);
            });
        }

        private void UserCanSelectObjectHud(IWorkspace workspace)
        {
            workspace.UseHud("user-can-select-object-hud", root =>
            {
                root.Q<Button>("done").clicked += () => workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
            });

        }
    }
}