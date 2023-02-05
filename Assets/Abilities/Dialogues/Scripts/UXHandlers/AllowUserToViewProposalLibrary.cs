using System;
using System.Collections.Generic;
using System.Linq;
using Pladdra.ARSandbox.Dialogues.Data;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToViewProposalLibrary: DialoguesUXHandler
    {
        public AllowUserToViewProposalLibrary(DialoguesUXManager uxManager)
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
                            IUXHandler ux = new AllowUserToViewProposal(uxManager);
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
                        button.Q<Button>("proposal-button").clicked += () =>
                        {
                            int i = (int)button.Q<Button>("proposal-button").userData;
                            actions[actions.Keys.ToArray()[i]]();
                        };
                        return button;
                    };
                    menulist.bindItem = (element, i) =>
                    {
                        Button button = element.Q<Button>();
                        button.text = actions.Keys.ToArray()[i].name;
                        button.userData = i;
                        // button.clicked += () => { actions[actions.Keys.ToArray()[i]](); };
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
