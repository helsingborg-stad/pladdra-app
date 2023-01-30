using System;
using System.Collections.Generic;
using System.Linq;
using Pladdra.DialogueAbility.Data;
using Pladdra.UX;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToViewProposalLibrary : UXHandler
    {
        public AllowUserToViewProposalLibrary(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            // TODO Clean up and move to project object
            Dictionary<Proposal, Action> actions = new Dictionary<Proposal, Action>();
            if (uxManager.Project.proposals != null && uxManager.Project.proposals.Count > 0)
            {
                foreach (var proposal in uxManager.Project.proposals)
                {
                    if (proposal != null && proposal.name != "working-proposal" && !actions.ContainsKey(proposal))
                    {
                        actions.Add(proposal, () =>
                        {
                            uxManager.Project.ProposalHandler.ShowProposal(proposal.name);
                            UXHandler ux = new AllowUserToViewProposal(uxManager);
                            uxManager.UseUxHandler(ux);
                        });
                    }
                }
            }

            uxManager.UIManager.DisplayUI("proposal-library", root =>
            {
                root.Q<Button>("continue").clicked += () =>
                {
                    uxManager.ShowWorkspaceDefault();
                };
                if (uxManager.Project.ProposalHandler.HasActiveProposal())
                {
                    root.Q<Button>("new-proposal").clicked += () =>
                    {
                        uxManager.Project.ProposalHandler.NewProposal();
                        uxManager.ShowWorkspaceDefault();
                    };
                }
                else
                {
                    root.Q<Button>("new-proposal").visible = false;
                    root.Q<Button>("continue").text = "Börja bygga"; //TODO Move to UItext
                }

                ListView menulist = root.Q<ListView>("proposals");
                if (actions.Count > 0)
                {
                    menulist.makeItem = () =>
                    {
                        var button = uxManager.UIManager.uiAssets.Find(x => x.name == "proposal-button-template").visualTreeAsset.Instantiate();

                        return button;
                    };
                    menulist.bindItem = (element, i) =>
                    {
                        Button button = element.Q<Button>();
                        button.text = actions.Keys.ToArray()[i].name;
                        button.clicked += () => { actions[actions.Keys.ToArray()[i]](); };
                    };
                    menulist.fixedItemHeight = 50;
                    menulist.itemsSource = actions.Keys.ToArray();
                }
                else
                {
                    menulist.visible = false;
                    menulist.style.height = 0;
                }

            });
        }
        public override void Deactivate()
        {

        }
    }
}
