using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Pladdra.ARSandbox.Dialogues.Data;

using System.Linq;
using Pladdra.UX;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToViewResourceLibrary: DialoguesUXHandler
    {
        public AllowUserToViewResourceLibrary(DialoguesUXManager uxManager)
        {
            this.uxManager = uxManager;
            this.project = uxManager.Project;
            // Debug.Log($"Created AllowUserToViewResourceLibrary with project {project.name}");
        }
        public override void Activate()
        {
            Debug.Log("Activating AllowUserToViewResourceLibrary");
            List<DialogueResource> libraryResources = project.resources.Where(r => r.displayRule == ResourceDisplayRules.Library).ToList();
            Dictionary<DialogueResource, Action> actions = new Dictionary<DialogueResource, Action>();
            if (libraryResources == null || libraryResources.Count == 0)
            {
                Debug.Log("No resources found");
                uxManager.SetUXToNull();
                uxManager.UIManager.ShowError("default", new string[] { "No resources found" });
                return;
            }
            foreach (var resource in libraryResources)
            {
                actions.Add(resource, () =>
                {
                    IUXHandler ux = new AllowUserToInspectModel(uxManager, resource);
                    uxManager.UseUxHandler(ux);
                });
            }
            uxManager.UIManager.DisplayUI("resource-library", root =>
            {
                root.Q<Button>("close").clicked += () => { uxManager.ShowWorkspaceDefault(); };
                VisualElement resourceList = root.Q<VisualElement>("resource-list");
                foreach (var resource in actions.Keys)
                {
                    var button = uxManager.UIManager.uiAssets.Find(x => x.name == "resource-button-template").visualTreeAsset.Instantiate();

                    resourceList.Add(button);
                    button.Q<Label>("name").text = resource.name.ToUpper();
                    button.Q<VisualElement>("image").style.backgroundImage = resource.thumbnail;
                    button.Q<Button>("button").clicked += () => { actions[resource](); };
                    resourceList.RegisterCallback<GeometryChangedEvent>(e =>
                    {
                        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                        {
                            button.style.width = (resourceList.resolvedStyle.width - (resourceList.resolvedStyle.paddingLeft + resourceList.resolvedStyle.paddingRight)) / 5.1f;
                            button.style.height = button.resolvedStyle.width;
                            button.Q<Label>("name").style.fontSize = button.resolvedStyle.height / 10;
                        }
                        else
                        {
                            button.style.width = (resourceList.resolvedStyle.width - (resourceList.resolvedStyle.paddingLeft + resourceList.resolvedStyle.paddingRight)) / 3.1f;
                            button.style.height = button.resolvedStyle.width;
                            button.Q<Label>("name").style.fontSize = button.resolvedStyle.height / 10;
                        }
                    });
                }
            });
        }

        public override void Deactivate()
        {
            
        }
    }
}