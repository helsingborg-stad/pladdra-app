using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Abilities.ARRoomAbility;
using UnityEngine;
using UnityEngine.UIElements;
using Utility; //TODO Change namespace name to Pladdra.Utility
using Pladdra.Data;
using UXHandlers;
using Pladdra.Workspace;

namespace Pladdra
{

    public class ProjectManager : MonoBehaviour
    {
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected WorkspaceManager workspaceManager { get { return transform.parent.gameObject.GetComponentInChildren<WorkspaceManager>(); } }
        protected IAbility ability; //TODO: Remove/Change terminology to ProjectReference 
        protected IUxHandler uxHandler = new NullUxHandler();
        protected Project project; 
        protected WorkspaceConfiguration configuration;
        protected Transform origin;
        public GameObject Origin { get { return origin.gameObject ?? (origin = new GameObject().transform).gameObject; } }
        public void LoadProject(IAbility ability)
        {
            this.ability = ability;

            // this action is updated to map to a label in our HUD
            Action<string> setLabelText = s => { };
            Action updateUI = () => { };
            var actions = new List<string>();

            // show progress UI
            uiManager.ShowUI("loading-project", root =>
            {
                var labelElement = root.Q<Label>("label");
                setLabelText = s => labelElement.text = s;
                updateUI = () => labelElement.text = string.Join("\r\n", actions);
            });

            // TODO Simplify ArRoomWorkspaceLoader to a ProjectLoader without all the fluff
            var pipeline = new ArRoomWorkspaceLoader(ability.Repository);
            pipeline.OnTaskStarted += label =>
            {
                actions.Add(label);
                updateUI();
            };

            // run project loading pipeline
            StartCoroutine(pipeline.LoadWorkspace((configuration, project) =>
            {
                this.project = project;
                this.configuration = configuration;
                InitProject();
            }).Catch(e =>
            {
                uiManager.ShowError(e.Message);
            }));
        }

        public void InitProject()
        {
            Debug.Log("Init Project " + project.Name);
            //TODO Show project info UI?
            if (project.markerRequired)
            {
                Debug.Log("Project requires marker");
                uiManager.ShowUI("look-for-marker");
                // TODO Add listener to marker found event
            }
            else
            {
                InstantiateProject();
            }
        }

        public void InstantiateProject()
        {
            Debug.Log("InstantiateProject");
            project.InstantiateStaticResources(origin);

            if (project.allowAnyUserToInteract == true && project.Resources.Count > 0 && project.UserProposals.Count == 0)
            {
                ShowWorkspace();
            }
            else
            {
                
            } // TODO 
        }

        public void ShowUserProposals()
        {
            // project.ShowUserProposals(); // TODO
        }

        public void HideUserProposals()
        {
            project.HideUserProposals();
        }
        public void ShowWorkspace()
        {
            Debug.Log("Show Workspace");
            project.ShowInteractiveResources(workspaceManager,ability,configuration,origin);
        }

        public void HideWorkspace()
        {
            project.HideInteractiveResources(workspaceManager);
        }

        public void SetOrigin(Transform origin)
        {
            this.origin = origin;
        }

    }
}