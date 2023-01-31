using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
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
            uxManager.UIManager.DisplayUI("workspace-default", root =>
                         {

                             if (uxManager.Project.hasLibraryResources)
                             {
                                 root.Q<Button>("add-item").clicked += () =>
                                 {
                                     UXHandler ux = new AllowUserToViewResourceLibrary(uxManager);
                                     uxManager.UseUxHandler(ux);
                                 };
                             }
                             else
                             {
                                 root.Q<Button>("add-item").visible = false;
                             }
                             root.Q<Button>("reposition-workspace").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToManipulateWorkspace(uxManager);
                                 uxManager.UseUxHandler(ux);
                             };
                         });

            uxManager.RaycastManager.OnHitObject.AddListener(uxManager.SelectObject);


        }
        public override void Deactivate()
        {
            uxManager.RaycastManager.OnHitObject.RemoveListener(uxManager.SelectObject);
        }
    }
}
