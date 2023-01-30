using Pladdra.UX;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToViewProposal : UXHandler
    {
        public AllowUserToViewProposal(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.DisplayUI("view-proposal", root =>
            {
                root.Q<Label>("name").text = uxManager.Project.ProposalHandler.Proposal.name;
                root.Q<Button>("close").clicked += () =>
                {
                    uxManager.Project.ProposalHandler.HideProposal();
                    UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
                root.Q<Button>("reposition-proposal").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToManipulateProposal(uxManager);
                                 uxManager.UseUxHandler(ux);
                             };
            });
        }
        public override void Deactivate()
        {

        }
    }
}
