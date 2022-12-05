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
    public class AllowUserToViewProject : UXHandler
    {
        public AllowUserToViewProject(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            Debug.Log("Init Project " + interactionManager.Project.name);
            if (interactionManager.Project.markerRequired)
            {
                Debug.Log("Project requires marker");
                interactionManager.UIManager.ShowUI("look-for-marker");
                // TODO Hook up listeners to marker found event
            }
            else
            {
                // TODO Add UITexts
                Dictionary<string, Action> actions = new Dictionary<string, Action>();
                if (interactionManager.Project.UserCanInteract())
                {
                    actions.Add("Börja bygga", () =>
                    {
                        interactionManager.ShowWorkspaceDefault();
                    });
                }
                if (interactionManager.Project.HasProposals())
                {
                    actions.Add("Se förslag", () =>
                    {
                        UXHandler ux = new AllowUserToViewProposalLibrary(interactionManager);
                        interactionManager.UseUxHandler(ux);
                    });
                }
                interactionManager.UIManager.ShowUI("project-info", root =>
                        {
                            root.Q<Label>("Name").text = interactionManager.Project.name;
                            root.Q<Label>("Description").text = interactionManager.Project.description;

                            Button start = root.Q<Button>("start");
                            start.clicked += () => 
                            {
                                interactionManager.ShowWorkspaceDefault();
                            };

                            Button suggestions = root.Q<Button>("suggestions");
                            suggestions.clicked += () =>
                            {
                                UXHandler ux = new AllowUserToViewProposalLibrary(interactionManager);
                                interactionManager.UseUxHandler(ux);
                            };
                            
                        }
            );
            }
        }
        public override void Deactivate()
        {

        }
    }
}
