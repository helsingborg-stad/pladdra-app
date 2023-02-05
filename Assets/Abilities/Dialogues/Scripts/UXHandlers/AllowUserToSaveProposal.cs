
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToSaveProposal: DialoguesUXHandler
    {
        public AllowUserToSaveProposal(DialoguesUXManager uxManager)
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
                                IUXHandler ux = new AllowUserToChooseActionAfterSaving(uxManager);
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
