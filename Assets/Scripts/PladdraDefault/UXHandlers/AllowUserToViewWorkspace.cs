using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToViewWorkspace : UXHandler
    {
        public AllowUserToViewWorkspace(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            interactionManager.UIManager.ShowUI("workspace-default", root =>
                         {
                             root.Q<Button>("add-item").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToViewResourceLibrary(interactionManager);
                                 interactionManager.UseUxHandler(ux);
                             };
                             root.Q<Button>("reposition-workspace").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToManipulateWorkspace(interactionManager);
                                 interactionManager.UseUxHandler(ux);
                             };
                             //  root.Q<Button>("undo").clicked += () =>
                             //  {

                             //  };
                             //  root.Q<Button>("redo").clicked += () =>
                             //  {

                             //  };
                         });

            interactionManager.RaycastManager.OnHitObject.AddListener(interactionManager.SelectObject);
        }
        public override void Deactivate()
        {
            interactionManager.RaycastManager.OnHitObject.RemoveListener(interactionManager.SelectObject);
        }
    }
}
