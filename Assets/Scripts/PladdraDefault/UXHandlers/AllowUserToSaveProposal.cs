using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToSaveProposal : UXHandler
    {
        public AllowUserToSaveProposal(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            interactionManager.UIManager.ShowUI("save-proposal", root =>
                        {
                            root.Q<Button>("confirm-button").clicked += () =>
                            {
                                string name = root.Q<TextField>("input-text").value;
                                UXHandler ux = new AllowUserToChooseActionAfterSaving(interactionManager);
                                interactionManager.UseUxHandler(ux);
                                Debug.Log("Saving proposal with name: " + name);
                                interactionManager.ProposalManager.SaveProposal(name);
                            };
                            root.Q<Button>("reject-button").clicked += () =>
                            {
                                interactionManager.UIManager.ShowPreviousUI();
                            };
                        });
        }
        public override void Deactivate()
        {

        }
    }
}
