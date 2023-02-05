
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToChooseActionAfterSaving: DialoguesUXHandler
    {
        public AllowUserToChooseActionAfterSaving(DialoguesUXManager uxManager)
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
                    IUXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
            });
        }
        public override void Deactivate()
        {
           
        }
    }
}
