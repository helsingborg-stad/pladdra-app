using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToManipulateWorkspace : UXHandler
    {
        WorkspaceController controller;
        float oldMinTouchLimit;

        public AllowUserToManipulateWorkspace(UXManager uxManager)
        {
            this.uxManager = uxManager;
            this.controller = uxManager.Project.WorkspaceController;
        }
        public override void Activate()
        {
            controller.Select();

            uxManager.UIManager.ShowUI("workspace-manipulate", root =>
            {
                //scale slider init
                var scaleSlider = root.Q<Slider>("scale");
                var sliderLabel = root.Q<Label>("scale-label");
                sliderLabel.text = $"1:{1 / scaleSlider.value}";
                root.Q<Button>("one").clicked += () =>
                {
                    controller.Scale(1f);
                    scaleSlider.value = 1f;
                    sliderLabel.text = $"1:{1 / scaleSlider.value}";
                };
                root.Q<Button>("hundred").clicked += () =>
                {
                    controller.Scale(0.01f);
                    scaleSlider.value = 0.01f;
                    sliderLabel.text = $"1:{1 / scaleSlider.value}";
                };
                scaleSlider.RegisterCallback<ChangeEvent<float>>(e =>
                {
                    controller.Scale(e.newValue);
                    sliderLabel.text = $"1:{1 / e.newValue}";
                });

                //rotation slider init
                var rotationSlider = root.Q<Slider>("rotation");
                var rotationLabel = root.Q<Label>("rotation-label");
                rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                root.Q<Button>("plusone").clicked += () =>
                {
                    controller.SetRotation(1f - 180f);
                    rotationSlider.value += 1f;
                    rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                };
                root.Q<Button>("minusone").clicked += () =>
                {
                    controller.SetRotation(-1f - 180f);
                    rotationSlider.value -= 1f;
                    rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                };
                rotationSlider.RegisterCallback<ChangeEvent<float>>(e =>
                {
                    controller.SetRotation(e.newValue - 180f);
                    rotationLabel.text = $"{controller.transform.rotation.eulerAngles.y - 180f}°";
                });

                //done button init
                root.Q<Button>("done").clicked += () =>
                {
                    this.Return();
                };
            });

            // TODO Maybe move these to the controller?
            uxManager.RaycastManager.LayerMask |= (1 << LayerMask.NameToLayer("ARMesh"));
            uxManager.RaycastManager.LayerMask &= ~(1 << controller.gameObject.layer);
            uxManager.RaycastManager.OnHitPoint.AddListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.AddListener(controller.FinalizeMove);
            uxManager.RaycastManager.OnTwoFingerTouch.AddListener(controller.Rotate);
            uxManager.RaycastManager.OnSecondFingerEnd.AddListener(controller.FinalizeRotation);
            oldMinTouchLimit = uxManager.RaycastManager.minTouchLimit;
            uxManager.RaycastManager.minTouchLimit = 600;
        }

        protected virtual void Return()
        {
            UXHandler ux = new AllowUserToViewWorkspace(uxManager);
            uxManager.UseUxHandler(ux);
        }
        public override void Deactivate()
        {
            uxManager.RaycastManager.LayerMask &= ~(1 << LayerMask.NameToLayer("ARMesh"));
            uxManager.RaycastManager.LayerMask |= (1 << controller.gameObject.layer);
            uxManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            uxManager.RaycastManager.OnEndTouch.RemoveListener(controller.FinalizeMove);
            uxManager.RaycastManager.OnTwoFingerTouch.RemoveListener(controller.Rotate);
            uxManager.RaycastManager.OnSecondFingerEnd.RemoveListener(controller.FinalizeRotation);
            uxManager.RaycastManager.minTouchLimit = oldMinTouchLimit;


            controller.FinalizeReposition();

            controller.Deselect();
        }
    }
}