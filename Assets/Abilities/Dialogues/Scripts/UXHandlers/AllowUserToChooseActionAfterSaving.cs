using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToChooseActionAfterSaving : UXHandler
    {
        public AllowUserToChooseActionAfterSaving(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.DisplayUI("save-confirmation", root =>
            {
                root.Q<Button>("continue").clicked += () =>
                {
                    uxManager.ShowWorkspaceDefault();
                };
                root.Q<Button>("proposal-library").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
            });
        }
        public override void Deactivate()
        {
           
        }
    }
}
