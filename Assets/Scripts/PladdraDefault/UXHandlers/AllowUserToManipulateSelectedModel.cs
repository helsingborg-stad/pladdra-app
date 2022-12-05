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
        public AllowUserToManipulateSelectedModel(InteractionManager interactionManager, PlacedObjectController controller, bool skipFirstEndTouch = true)
        {
            this.interactionManager = interactionManager;
            this.controller = controller;
            this.skipFirstEndTouch = skipFirstEndTouch;
        }
        public override void Activate()
        {
            controller.Select();

            interactionManager.UIManager.ShowUI("workspace-with-selected-object", root =>
                         {
                            Label rotationLabel = root.Q<Label>("rotation-label");
                            Slider rotationSlider = root.Q<Slider>("rotation-slider");
                            rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y}°";

                             root.Q<Button>("plus-ninety").clicked += () =>
                             {
                                 controller.SetRotation(controller.transform.rotation.eulerAngles.y + 90f);
                                 rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y}°";
                                 rotationSlider.value = Mathf.InverseLerp(360f, 0f, controller.transform.rotation.eulerAngles.y);
                             };

                             root.Q<Button>("minus-ninety").clicked += () =>
                             {
                                 controller.SetRotation(controller.transform.rotation.eulerAngles.y - 90f);
                                 rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y}°";
                                 rotationSlider.value = Mathf.InverseLerp(360f, 0f, controller.transform.rotation.eulerAngles.y);
                             };

                            root.Q<Slider>("rotation-slider").RegisterValueChangedCallback(e =>
                            {
                                float remappedValue = Mathf.Lerp(-180f, 180f, e.newValue);
                                controller.SetRotation(remappedValue);
                                rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y}°";
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
                                 UXHandler ux = new AllowUserToViewWorkspace(interactionManager);
                                 interactionManager.UseUxHandler(ux);
                             };
                             root.Q<Button>("done").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToViewWorkspace(interactionManager);
                                 interactionManager.UseUxHandler(ux);
                             };
                         });

            // Remove currently selected object's layer from the layermask so we don't place it on itself
            interactionManager.RaycastManager.LayerMask &= ~(1 << controller.gameObject.layer);

            interactionManager.RaycastManager.OnHitPoint.AddListener(controller.Move);
            interactionManager.RaycastManager.OnEndTouch.AddListener(Deselect);
            interactionManager.RaycastManager.OnTwoFingerTouch.AddListener(controller.Rotate);
            interactionManager.RaycastManager.OnSecondFingerEnd.AddListener(controller.FinalizeRotation);
        }

        void Deselect()
        {
            if (skipFirstEndTouch)
            {
                // Workaround to avoid listening to the first end touch event
                skipFirstEndTouch = false;
                return;
            }
            interactionManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            interactionManager.RaycastManager.OnEndTouch.RemoveListener(Deselect);
            interactionManager.RaycastManager.OnTwoFingerTouch.RemoveListener(controller.Rotate);
            interactionManager.RaycastManager.OnSecondFingerEnd.RemoveListener(controller.FinalizeRotation);

            controller.FinalizeReposition();

            controller.Deselect();
            UXHandler ux = new AllowUserToViewWorkspace(interactionManager);
            interactionManager.UseUxHandler(ux);
        }
        public override void Deactivate()
        {
            // Return the layer so we can reselect it
            interactionManager.RaycastManager.LayerMask |= (1 << controller.gameObject.layer);
            interactionManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
        }
    }
}
