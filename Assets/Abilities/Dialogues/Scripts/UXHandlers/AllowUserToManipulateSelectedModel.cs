
using Pladdra.UX;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToManipulateSelectedModel: DialoguesUXHandler
    {
        InteractiveObjectController controller;
        bool skipFirstEndTouch = true;
        public AllowUserToManipulateSelectedModel(DialoguesUXManager uxManager, InteractiveObjectController controller, bool skipFirstEndTouch = true)
        {
            this.uxManager = uxManager;
            this.controller = controller;
            this.skipFirstEndTouch = skipFirstEndTouch;
        }
        public override void Activate()
        {
            controller.Select();

            uxManager.UIManager.DisplayUI("workspace-with-selected-object", root =>
                        {
                            Label rotationLabel = root.Q<Label>("rotation-label");
                            // controller.SetRotation(180f); // ??? 
                            Slider rotationSlider = root.Q<Slider>("rotation-slider");
                            rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";

                            root.Q<Button>("plus-ninety").clicked += () =>
                            {
                                controller.SetRotation(controller.transform.rotation.eulerAngles.y - 180f + 90f);
                                if ((controller.transform.rotation.eulerAngles.y - 180f) > 180f)
                                {
                                    controller.SetRotation(-180f);
                                    rotationSlider.value = -180f;
                                }
                                rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                                rotationSlider.value += 90f;
                            };

                            root.Q<Button>("minus-ninety").clicked += () =>
                            {
                                controller.SetRotation(controller.transform.rotation.eulerAngles.y - 180f - 90f);
                                if ((controller.transform.rotation.eulerAngles.y - 180f) < -180f)
                                {
                                    controller.SetRotation(180f);
                                    rotationSlider.value = 180f;
                                }
                                rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                                rotationSlider.value -= 90f;
                            };

                            root.Q<Slider>("rotation-slider").RegisterValueChangedCallback(e =>
                            {
                                controller.SetRotation(e.newValue - 180f);
                                rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                            });

                            //  root.Q<Button>("undo").clicked += () =>
                            //  {

                            //  };
                            //  root.Q<Button>("redo").clicked += () =>
                            //  {

                            //  };
                            root.Q<Button>("delete").clicked += () =>
                            {
                                Deselect();
                                controller.Delete();
                            };
                            root.Q<Button>("done").clicked += () =>
                            {
                                Deselect();
                            };
                        });

            // Remove currently selected object's layer from the layermask so we don't place it on itself
            uxManager.RaycastManager.SetLayerMask("allowUserToManipulateSelectedModel");

            uxManager.RaycastManager.OnHitPoint.AddListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.AddListener(Deselect);
            uxManager.RaycastManager.OnTwoFingerTouch.AddListener(controller.Rotate);
            uxManager.RaycastManager.OnSecondFingerEnd.AddListener(controller.FinalizeRotation);
        }

        void Deselect()
        {
            if (skipFirstEndTouch)
            {
                // Workaround to avoid listening to the first end touch event
                skipFirstEndTouch = false;
                return;
            }
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.RemoveListener(Deselect);
            uxManager.RaycastManager.OnTwoFingerTouch.RemoveListener(controller.Rotate);
            uxManager.RaycastManager.OnSecondFingerEnd.RemoveListener(controller.FinalizeRotation);

            controller.Deselect();
            IUXHandler ux = new AllowUserToViewWorkspace(uxManager);
            uxManager.UseUxHandler(ux);
        }
        public override void Deactivate()
        {
            // Return the layer so we can reselect it
            uxManager.RaycastManager.SetDefaultLayerMask();
            
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
        }
    }
}
