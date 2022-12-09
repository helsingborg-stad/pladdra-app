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
        public AllowUserToViewProject(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("Init Project " + uxManager.Project.name);
            if (uxManager.Project.markerRequired)
            {
                Debug.Log("Project requires marker");
                uxManager.UIManager.ShowUI("look-for-marker");
                // TODO Hook up listeners to marker found event
            }
            else
            {
                // TODO Add UITexts
                Dictionary<string, Action> actions = new Dictionary<string, Action>();
                if (uxManager.Project.UserCanInteract())
                {
                    actions.Add("Börja bygga", () =>
                    {
                        uxManager.ShowWorkspaceDefault();
                    });
                }
                if (uxManager.Project.HasProposals())
                {
                    actions.Add("Se förslag", () =>
                    {
                        UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                        uxManager.UseUxHandler(ux);
                    });
                }
                uxManager.UIManager.ShowUI("project-info", root =>
                        {
                            root.Q<Label>("Name").text = uxManager.Project.name;
                            root.Q<Label>("Description").text = uxManager.Project.description;

                            Button start = root.Q<Button>("start");
                            start.clicked += () => 
                            {
                                uxManager.ShowWorkspaceDefault();
                            };

                            Button suggestions = root.Q<Button>("suggestions");
                            suggestions.clicked += () =>
                            {
                                UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                                uxManager.UseUxHandler(ux);
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
