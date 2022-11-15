using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Pladdra.Workspace;
using Pladdra.Data;
using Pladdra.UI;
using Pladdra;

namespace Abilities.ARRoomAbility.UxHandlers
{
    public class AllowUserToViewResourceLibrary : AbstractUxHandler
    {
        protected Project project;
        protected InteractionManager interactionManager;

        // TODO Move this down a level
        public AllowUserToViewResourceLibrary(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
            this.project = interactionManager.Project;
            Debug.Log($"Created AllowUserToViewResourceLibrary with project {project.Name}");
        }

        public override void Activate(UIManager uiManager, ProposalManager proposalManager)
        {
            Dictionary<PladdraResource, Action> actions = new Dictionary<PladdraResource, Action>();
            if(project.Resources == null || project.Resources.Count == 0)
            {
                Debug.Log("No resources found");
                return;
            }
            foreach (var resource in project.Resources)
            {
                actions.Add(resource, () => { proposalManager.AddItem(resource); });
            }
            uiManager.ShowUI("resource-library", root =>
            {
                root.Q<Button>("close").clicked += () => { interactionManager.ShowWorkspaceDefault(); };
                ListView listView = root.Q<ListView>("resource-list");
                listView.makeItem = () =>
                {
                    var button = uiManager.resourceButtonTemplate.Instantiate();
                    return button;
                };
                listView.bindItem = (element, i) =>
                {
                    element.Q<Button>("button").clicked += () =>
                    {
                        actions[actions.Keys.ElementAt(i)]();
                        uiManager.ShowPreviousUI();
                    };
                    element.Q<Label>("name").text = actions.Keys.ElementAt(i).Name;
                    element.Q<VisualElement>("image").style.backgroundImage = actions.Keys.ElementAt(i).Thumbnail;
                };
                listView.itemsSource = actions.Keys.ToArray();
            });
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return null;
        }
    }
}