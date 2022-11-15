using System;
using System.Collections;
using System.Collections.Generic;
using Abilities.ARRoomAbility.UxHandlers;
using Pladdra.Data;
using UnityEngine;
using Pladdra.UI;
using UXHandlers;
using UnityEngine.UIElements;
using Pladdra.Workspace.EditHistory;

namespace Pladdra
{
    public class InteractionManager : MonoBehaviour
    {
        AbstractUxHandler uxHandler;

        Project project;
        public Project Project { get { return project; } set { project = value; } }

        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        ProposalManager proposalManager { get { return transform.parent.gameObject.GetComponentInChildren<ProposalManager>(); } }
        IWorkspaceEditHistory History { get; set; }

        void Start()
        {
            History = new WorkspaceEditHistory();
        }

        public void ShowWorkspaceDefault()
        {
            // TODO Change to UXHandler
            uiManager.ShowUI("workspace-default", root =>
                        {
                            root.Q<Button>("add-item").clicked += () =>
                            {
                                ShowResources();
                            };
                            root.Q<Button>("undo").clicked += () =>
                            {

                            };
                            root.Q<Button>("redo").clicked += () =>
                            {

                            };
                        });
        }

        // TODO Ideally we should generalize this and be able to create a new generic UXHandler from list
        void ShowResources()
        {
            uxHandler = new AllowUserToViewResourceLibrary(this);
            uxHandler.Activate(uiManager, proposalManager);
            //TODO Move to UX Handler
            // Dictionary<string, Action> actions = new Dictionary<string, Action>();
            // foreach (var resource in project.Resources)
            // {
            //     actions.Add(resource.Name, () => { AddItem(resource); });
            // }
            // uiManager.ShowUI("resources-library", root =>
            // {
            //     root.Q<Button>("close").clicked += () => { uiManager.ShowPreviousUI(); };
            //     ListView listView = root.Q<ListView>("resources-list");
            //     listView.makeItem = () =>
            //     {
            //         var button = uiManager.resourceButtonTemplate.Instantiate();
            //         return button;
            //     };
            //     listView.bindItem = (element, i) =>
            //     {
            //         var button = element.Q<Button>();
            //         button.text = actions.Keys.ElementAt(i);
            //         button.clicked += () =>
            //         {
            //             actions[actions.Keys.ElementAt(i)]();
            //             uiManager.ShowPreviousUI();
            //         };
            //     };
            //     listView.itemsSource = actions.Keys.ToArray();
            // });
        }
    }
}