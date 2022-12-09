using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Pladdra.Workspace;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToViewResourceLibrary : UXHandler
    {
        public AllowUserToViewResourceLibrary(UXManager uxManager)
        {
            this.uxManager = uxManager;
            this.project = uxManager.Project;
            Debug.Log($"Created AllowUserToViewResourceLibrary with project {project.name}");
        }
        public override void Activate()
        {
            Debug.Log("Activating AllowUserToViewResourceLibrary");
            Dictionary<PladdraResource, Action> actions = new Dictionary<PladdraResource, Action>();
            if (project.resources == null || project.resources.Count == 0)
            {
                Debug.Log("No resources found");
                uxManager.SetUXToNull();
                uxManager.UIManager.ShowError("default", new string[] { "No resources found" });
                return;
            }
            foreach (var resource in project.resources)
            {
                actions.Add(resource, () =>
                {
                    UXHandler ux = new AllowUserToInspectModel(uxManager, resource);
                    uxManager.UseUxHandler(ux);
                });
            }
            uxManager.UIManager.ShowUI("resource-library", root =>
            {
                root.Q<Button>("close").clicked += () => { uxManager.ShowWorkspaceDefault(); };
                VisualElement resourceList = root.Q<VisualElement>("resource-list");
                foreach(var resource in actions.Keys)
                {
                    var button = uxManager.UIManager.resourceButtonTemplate.Instantiate();
                    resourceList.Add(button);
                    button.Q<Label>("name").text = resource.name.ToUpper();
                    button.Q<VisualElement>("image").style.backgroundImage = resource.thumbnail;
                    button.Q<Button>("button").clicked += () => { actions[resource](); };
                    resourceList.RegisterCallback<GeometryChangedEvent>(e =>
                    {
                        // Debug.Log($"Device orientation: {Input.deviceOrientation}");
                        if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
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
            // TODO
        }
    }
}