using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToSaveProposal : UXHandler
    {
        public AllowUserToSaveProposal(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.DisplayUI("save-proposal", root =>
                        {
                            root.Q<Button>("confirm-button").clicked += () =>
                            {
                                string name = root.Q<TextField>("input-text").value;
                                UXHandler ux = new AllowUserToChooseActionAfterSaving(uxManager);
                                uxManager.UseUxHandler(ux);
                                Debug.Log("Saving proposal with name: " + name);
                                uxManager.Project.ProposalHandler.SaveProposal(name);
                            };
                            root.Q<Button>("reject-button").clicked += () =>
                            {
                                uxManager.UseLastUX();
                            };
                        });
        }
        public override void Deactivate()
        {

        }
    }
}
