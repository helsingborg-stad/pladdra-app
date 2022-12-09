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
        public AllowUserToSaveProposal(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.ShowUI("save-proposal", root =>
                        {
                            root.Q<Button>("confirm-button").clicked += () =>
                            {
                                string name = root.Q<TextField>("input-text").value;
                                UXHandler ux = new AllowUserToChooseActionAfterSaving(uxManager);
                                uxManager.UseUxHandler(ux);
                                Debug.Log("Saving proposal with name: " + name);
                                uxManager.ProposalManager.SaveProposal(name);
                            };
                            root.Q<Button>("reject-button").clicked += () =>
                            {
                                uxManager.UIManager.ShowPreviousUI();
                            };
                        });
        }
        public override void Deactivate()
        {

        }
    }
}
