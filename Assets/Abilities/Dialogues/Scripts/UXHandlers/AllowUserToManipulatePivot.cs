
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToManipulatePivot: DialoguesUXHandler
    {
        PivotController controller;
        bool skipFirstEndTouch = true;
        LayerMask oldLayerMask;

        public AllowUserToManipulatePivot(DialoguesUXManager uxManager, PivotController controller, bool skipFirstEndTouch = true)
        {
            this.uxManager = uxManager;
            this.controller = controller;
            this.skipFirstEndTouch = skipFirstEndTouch;
        }
        
        public override void Activate()
        {
            controller.Select();
            controller.ToggleVisibility(true);
            oldLayerMask = uxManager.RaycastManager.LayerMask;

            uxManager.UIManager.DisplayUI("workspace-manipulate-pivot", root =>
                {
                    root.Q<Button>("done").clicked += () =>
                    {
                        Deselect();
                        // UXHandler ux = new AllowUserToManipulateWorkspace(uxManager);
                        // uxManager.UseUxHandler(ux);
                    };
                });

            uxManager.RaycastManager.SetLayerMask("allowUserToManipulatePivot");
            // uxManager.RaycastManager.LayerMask = LayerMask.GetMask("StaticResources");
            uxManager.RaycastManager.OnHitPoint.AddListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.AddListener(Deselect);
        }

        void Deselect()
        {
            // if (skipFirstEndTouch)
            // {
            //     // Workaround to avoid listening to the first end touch event
            //     skipFirstEndTouch = false;
            //     return;
            // }
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.RemoveListener(Deselect);

            controller.Deselect();
            IUXHandler ux = new AllowUserToManipulateWorkspace(uxManager);
            uxManager.UseUxHandler(ux);
        }

        public override void Deactivate()
        {
            // Return the layer so we can reselect it
            // uxManager.RaycastManager.LayerMask = oldLayerMask;
            controller.ToggleVisibility(false);

            uxManager.RaycastManager.SetDefaultLayerMask();
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.RemoveListener(Deselect);

            // controller.gameObject.layer = LayerMask.NameToLayer("ScalePivot");
        }
    }
}
