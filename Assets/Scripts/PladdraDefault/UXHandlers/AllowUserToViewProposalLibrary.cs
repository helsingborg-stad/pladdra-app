using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToViewProposalLibrary : UXHandler
    {
        public AllowUserToViewProposalLibrary(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            Dictionary<Proposal, Action> actions = new Dictionary<Proposal, Action>();
            if (interactionManager.Project.proposals != null && interactionManager.Project.proposals.Count > 0)
            {
                foreach (var proposal in interactionManager.Project.proposals)
                {
                    if (proposal != null)
                    {
                        actions.Add(proposal, () =>
                        {
                            interactionManager.ProposalManager.ShowProposal(proposal.name);
                            UXHandler ux = new AllowUserToViewProposal(interactionManager);
                            interactionManager.UseUxHandler(ux);
                        });
                    }
                    else
                    {
                        Debug.LogError("Proposal is null");
                    }
                }
            }

            interactionManager.UIManager.ShowUI("proposal-library", root =>
            {
                root.Q<Button>("continue").clicked += () =>
                {
                    interactionManager.ShowWorkspaceDefault();
                };
                if (interactionManager.ProposalManager.HasActiveProposal())
                {
                    root.Q<Button>("new-proposal").clicked += () =>
                    {
                        interactionManager.ProposalManager.NewProposal();
                        interactionManager.ShowWorkspaceDefault();
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
                        var button = interactionManager.UIManager.proposalButtonTemplate.Instantiate();
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
