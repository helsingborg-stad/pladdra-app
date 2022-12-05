using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToViewProposal : UXHandler
    {
        public AllowUserToViewProposal(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            interactionManager.UIManager.ShowUI("view-proposal", root =>
            {
                root.Q<Label>("name").text = interactionManager.ProposalManager.Proposal.name;
                root.Q<Button>("close").clicked += () =>
                {
                    interactionManager.ProposalManager.HideProposal();
                    UXHandler ux = new AllowUserToViewProposalLibrary(interactionManager);
                    interactionManager.UseUxHandler(ux);
                };
                root.Q<Button>("reposition-proposal").clicked += () =>
                             {
                                 UXHandler ux = new AllowUserToManipulateProposal(interactionManager);
                                 interactionManager.UseUxHandler(ux);
                             };
            });
        }
        public override void Deactivate()
        {

        }
    }
}
