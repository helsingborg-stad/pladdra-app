using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToManipulateSelectedModel : UXHandler
    {
        PlacedObjectController controller;
        bool skipFirstEndTouch = true;
        public AllowUserToManipulateSelectedModel(UXManager uxManager, PlacedObjectController controller, bool skipFirstEndTouch = true)
        {
            this.uxManager = uxManager;
            this.controller = controller;
            this.skipFirstEndTouch = skipFirstEndTouch;
        }
        public override void Activate()
        {
            controller.Select();

            uxManager.UIManager.ShowUI("workspace-with-selected-object", root =>
                        {
                            Label rotationLabel = root.Q<Label>("rotation-label");
                            controller.SetRotation(180f);
                            Slider rotationSlider = root.Q<Slider>("rotation-slider");
                            rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";

                            root.Q<Button>("plus-ninety").clicked += () =>
                            {
                                controller.SetRotation(controller.transform.rotation.eulerAngles.y - 180f + 90f);
                                if((controller.transform.rotation.eulerAngles.y - 180f) > 180f)
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
                                if((controller.transform.rotation.eulerAngles.y - 180f) < -180f)
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
                                controller.Delete();
                                UXHandler ux = new AllowUserToViewWorkspace(uxManager);
                                uxManager.UseUxHandler(ux);
                            };
                            root.Q<Button>("done").clicked += () =>
                            {
                                UXHandler ux = new AllowUserToViewWorkspace(uxManager);
                                uxManager.UseUxHandler(ux);
                            };
                        });

            // Remove currently selected object's layer from the layermask so we don't place it on itself
            uxManager.RaycastManager.LayerMask &= ~(1 << controller.gameObject.layer);

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

            controller.FinalizeReposition();

            controller.Deselect();
            UXHandler ux = new AllowUserToViewWorkspace(uxManager);
            uxManager.UseUxHandler(ux);
        }
        public override void Deactivate()
        {
            // Return the layer so we can reselect it
            uxManager.RaycastManager.LayerMask |= (1 << controller.gameObject.layer);
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
        }
    }
}
