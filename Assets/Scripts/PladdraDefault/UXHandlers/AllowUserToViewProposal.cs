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
        public AllowUserToViewProposal(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.ShowUI("view-proposal", root =>
            {
                root.Q<Label>("name").text = uxManager.ProposalManager.Proposal.name;
                root.Q<Button>("close").clicked += () =>
                {
                    uxManager.ProposalManager.HideProposal();
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
