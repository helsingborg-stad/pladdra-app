using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToManipulateWorkspace : UXHandler
    {
        WorkspaceController controller;
        public AllowUserToManipulateWorkspace(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
            this.controller = interactionManager.Project.workspaceController;
        }
        public override void Activate()
        {
            controller.Select();

            interactionManager.UIManager.ShowUI("workspace-manipulate", root =>
            {
                var slider = root.Q<Slider>("scale");
                var sliderLabel = root.Q<Label>("scale-label");
                sliderLabel.text = $"1:{1 / slider.value}";
                root.Q<Button>("one").clicked += () =>
                {
                    controller.Scale(1f);
                    slider.value = 1f;
                    sliderLabel.text = $"1:{1 / slider.value}";
                    // root.Q<Label>("current-scale").text = "1:1";
                };
                root.Q<Button>("hundred").clicked += () =>
                {
                    controller.Scale(0.01f);
                    slider.value = 0.01f;
                    sliderLabel.text = $"1:{1 / slider.value}";
                    // root.Q<Label>("current-scale").text = "1:100";
                };
                slider.RegisterCallback<ChangeEvent<float>>(e =>
                {
                    controller.Scale(e.newValue);
                    sliderLabel.text = $"1:{1 / e.newValue}";
                });
                root.Q<Button>("done").clicked += () =>
                {
                    this.Return();
                };
            });

            // TODO Maybe move these to the controller?
            interactionManager.RaycastManager.LayerMask |= (1 << LayerMask.NameToLayer("ARMesh"));
            interactionManager.RaycastManager.LayerMask &= ~(1 << controller.gameObject.layer);
            interactionManager.RaycastManager.OnHitPoint.AddListener(controller.Move);
            interactionManager.RaycastManager.OnTwoFingerTouch.AddListener(controller.Rotate);
            interactionManager.RaycastManager.OnSecondFingerEnd.AddListener(controller.FinalizeRotation);
        }

        protected virtual void Return()
        {
            UXHandler ux = new AllowUserToViewWorkspace(interactionManager);
            interactionManager.UseUxHandler(ux);
        }
        public override void Deactivate()
        {
            interactionManager.RaycastManager.LayerMask &= ~(1 << LayerMask.NameToLayer("ARMesh"));
            interactionManager.RaycastManager.LayerMask |= (1 << controller.gameObject.layer);
            interactionManager.RaycastManager.OnHitPoint.RemoveListener(controller.Move);
            interactionManager.RaycastManager.OnTwoFingerTouch.RemoveListener(controller.Rotate);
            interactionManager.RaycastManager.OnSecondFingerEnd.RemoveListener(controller.FinalizeRotation);

            controller.FinalizeReposition();

            controller.Deselect();
        }
    }
}