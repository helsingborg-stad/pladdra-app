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
        public AllowUserToViewWorkspace(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("AllowUserToViewWorkspace activated");
            uxManager.UIManager.ShowUI("workspace-default", root =>
                         {
                             root.Q<Button>("add-item").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToViewResourceLibrary(uxManager);
                                 uxManager.UseUxHandler(ux);
                             };
                             root.Q<Button>("reposition-workspace").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToManipulateWorkspace(uxManager);
                                 uxManager.UseUxHandler(ux);
                             };
                             //  root.Q<Button>("undo").clicked += () =>
                             //  {

                             //  };
                             //  root.Q<Button>("redo").clicked += () =>
                             //  {

                             //  };
                         });

            uxManager.RaycastManager.OnHitObject.AddListener(uxManager.SelectObject);
            
        }
        public override void Deactivate()
        {
            uxManager.RaycastManager.OnHitObject.RemoveListener(uxManager.SelectObject);
        }
    }
}
