using System.Collections.Generic;
using System.Linq;
using Lean.Common;
using Lean.Touch;
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
        private bool PinchToScale = false;
        private IGameObjectPositionInspector GameObjectPositionInspector { get; set; }

        public AllowUserToPositionObjects()
        {
            GameObjectPositionInspector = new NullGameObjectPositionInspector();
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);

            foreach (var go in GetSelectableObjects(scene))
            {
                BindDraggablesToLeanPlaneInstance(go, scene.Plane.gameObject.GetComponentInChildren<LeanPlane>());
                TryConfigureComponent<LeanPinchScale>(go, leanPinchScale => leanPinchScale.enabled = PinchToScale);
            }
            
            UserCanSelectObjectHud(workspace);
        }

        protected override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnSelected(scene, workspace, go);
            // add component that listens for position changes
            var tc = go.GetOrAddComponent<TransformChangedHandler>();
            tc.enabled = true;
            tc.OnPositionChanged.AddListener(position => GameObjectPositionInspector.OnPositionChanged(position));

            UseUserHasSelectedWorkspaceObjectHud(scene, workspace, go);
        }
        
        protected override void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnDeselected(scene, workspace, go);
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
                root.Q<Button>("gesture-behaviour-toggle").clicked += () =>
                {
                    PinchToScale = !PinchToScale;
                    TryConfigureComponent<LeanPinchScale>(go, leanPinchScale => leanPinchScale.enabled = PinchToScale);
                    TryConfigureComponent<LeanTwistRotateAxis>(go, leanPinchScale => leanPinchScale.enabled = !PinchToScale);
                    root.Q<Button>("gesture-behaviour-toggle").text = PinchToScale ? "Skala" : "Rotera";
                };
                root.Q<Button>("remove").clicked += () =>
                {
                    scene.ObjectsManager.DestroyItem(go);
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
                root.Q<Button>("done").clicked += () => workspace.Actions.DispatchAction("default");
            });

        }
        
        private void BindDraggablesToLeanPlaneInstance(GameObject go, LeanPlane leanPlane) => TryConfigureComponent<LeanDragTranslateAlong>(go, leanDragTranslateAlong => leanDragTranslateAlong.ScreenDepth.Object = leanPlane);

    }
}