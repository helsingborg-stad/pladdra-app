using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using System.Linq;
using Pladdra.DefaultAbility.UX;
using UnityEngine.Events;
using System;
using UntoldGarden.Utils;

namespace Pladdra.DefaultAbility
{
    public class ProposalManager : MonoBehaviour
    {
        #region Public
        public UnityEvent<string, string> OnSaveProposal = new UnityEvent<string, string>();
        #endregion Public

        #region Private
        ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }
        InteractionManager interactionManager { get { return transform.parent.gameObject.GetComponentInChildren<InteractionManager>(); } }
        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        Proposal proposal;
        public Proposal Proposal { get => proposal; }
        List<PlacedObjectController> placedObjects = new List<PlacedObjectController>();
        List<GameObject> loadedProposalObjects = new List<GameObject>();

        #endregion Private
        // TODO Make auto name 
        // TODO Prompt name on exit

        #region Proposal Objects

        /// <summary>
        /// Adds a new object to our proposal
        /// </summary>
        /// <param name="resource">The resource containing the object</param>
        /// <param name="position">The position of the object</param>
        /// <param name="controller">The controller of the object</param>
        public void AddObject(PladdraResource resource, Vector3 position, out PlacedObjectController controller)
        {
            Debug.Log($"Adding object {resource.Name}");

            GameObject obj = Instantiate(interactionManager.ObjectPrefab, projectManager.Project.projectContainer);
            obj.name = resource.Name;
            obj.transform.position = position;
            GameObject model = Instantiate(resource.Model, obj.transform);
            if (resource.Scale != Vector3.zero) model.transform.localScale = resource.Scale;
            controller = obj.GetComponent<PlacedObjectController>() ?? obj.AddComponent<PlacedObjectController>();
            controller.Init(this, interactionManager);
            controller.Resource = resource;
            placedObjects.Add(controller);
            model.SetActive(true);

            // Create a new proposal 
            if (proposal == null)
            {
                proposal = new Proposal();
                proposal.placedObjects = new List<ProposalResource>();
                uiManager.MenuManager.AddMenuItem(new MenuItem()
                {
                    id = "save-proposal",
                    name = "Spara förslag", // TODO Change to call from UItexts
                    action = () =>
                    {
                        UXHandler ux = new AllowUserToSaveProposal(interactionManager);
                        interactionManager.UseUxHandler(ux);
                    }
                });
            }

            Vector3 localPosition = model.transform.localPosition;
            Debug.Log($"ProposalManager: Adding {resource.Name} to proposal at {localPosition}");
            proposal.placedObjects.Add(new ProposalResource()
            {
                modelId = resource.ModelURL,
                localId = controller.Id,
                name = resource.Name,
                position = localPosition,
                rotation = Vector3.zero,
            });
            SaveWorkingProposalLocally();
        }

        public void AddObject(ProposalResource resource, Vector3 position, Vector3 rotation)
        {
            Debug.Log($"Adding object {resource.name} at {position} with rotation {rotation}");
            GameObject obj = projectManager.Project.resources.Find(x => x.ModelURL == resource.modelId).Model;
            GameObject model = Instantiate(obj, projectManager.Project.projectContainer);
            model.transform.localPosition = position;
            model.transform.rotation = Quaternion.Euler(rotation);
            model.SetActive(true);
            loadedProposalObjects.Add(model);
        }

        /// <summary>
        /// Removes an object from our proposal
        /// </summary>
        /// <param name="PlacedObjectController">The controller of the object to remove</param>
        public void RemoveObject(PlacedObjectController PlacedObjectController)
        {
            placedObjects.Remove(PlacedObjectController);
            Destroy(PlacedObjectController.gameObject);
        }

        /// <summary>
        /// Removes all objects from the proposal
        /// </summary>
        public void RemoveAllObjects()
        {
            foreach (PlacedObjectController placedObject in placedObjects)
            {
                Destroy(placedObject.gameObject);
            }
            placedObjects.Clear();
        }

        #endregion Proposal Objects

        #region Proposals

        /// <summary>
        /// Updates an object in the proposal
        /// </summary>
        /// <param name="id">Local ID of object to update</param>
        /// <param name="position">Position of object</param>
        /// <param name="y">Rotation Y of object</param>
        /// <param name="currentScale">Scale of object (uniform)</param>
        public void UpdateProposal(string id, Vector3 position, float y, float currentScale)
        {
            Debug.Log($"Updating proposal {proposal.placedObjects.Find(x => x.localId == id).name} at {position} with rotation {y} and scale {currentScale}");
            ProposalResource resource = proposal.placedObjects.Find(x => x.localId == id);
            resource.position = position;
            resource.rotation = new Vector3(0, y, 0);
            resource.scale = currentScale;
            SaveWorkingProposalLocally();
        }

        /// <summary>
        /// Displays a proposal
        /// </summary>
        /// <param name="proposal">Name of the proposal to show</param>
        public void ShowProposal(string proposal)
        {
            if (this.proposal != null)
            {
                NewProposal();
                //TODO Double check this, should we save it here?
            }
            this.proposal = projectManager.Project.proposals.Find(p => p.name == proposal);
            float scale = projectManager.Project.workspaceController.CurrentScale;
            projectManager.Project.workspaceController.Scale(1);
            foreach (ProposalResource resource in this.proposal.placedObjects)
            {
                AddObject(resource, resource.position, resource.rotation);
            }
            projectManager.Project.workspaceController.Scale(scale);
        }

        /// <summary>
        /// Hides a displayed proposal
        /// </summary>
        public void HideProposal()
        {
            foreach (GameObject obj in loadedProposalObjects)
            {
                Destroy(obj);
            }
            loadedProposalObjects.Clear();
            proposal = null;
        }

        /// <summary>
        /// Nulls the proposal and removes all its objects
        /// </summary>
        public void NewProposal()
        {
            proposal = null;
            RemoveAllObjects();
        }

        /// <summary>
        /// Saves proposal to database and locally
        /// </summary>
        /// <param name="name">Name of proposal</param>
        public void SaveProposal(string name)
        {
            if (proposal == null) return;
            proposal.name = name;
            projectManager.Project.proposals.Add(proposal);
            string proposalJSON = JsonUtility.ToJson(proposal);
            OnSaveProposal.Invoke(name, proposalJSON);
            SaveProposalLocally(name, proposalJSON);
            PlayerPrefs.DeleteKey(projectManager.Project.name + "-working-proposal"); // Remove the working proposal as the user has saved it
        }

        /// <summary>
        /// Saves proposal locally so it's easily available for the next session
        /// Overwrites any previously saved proposal with the same name
        /// </summary>
        public void SaveProposalLocally(string name, string json)
        {
            string proposals = "";
            if (PlayerPrefs.HasKey(projectManager.Project.name + "-proposals"))
            {
                proposals = PlayerPrefs.GetString(projectManager.Project.name + "-proposals");
            }
            proposals += name + ";";
            PlayerPrefs.SetString(projectManager.Project.name + "-proposals", proposals);
            PlayerPrefs.SetString(projectManager.Project.name + "-" + name, json);
        }

        /// <summary>
        /// Invoked on every model-related action
        /// </summary>
        public void SaveWorkingProposalLocally()
        {
            if (proposal == null) return;
            string proposalJSON = JsonUtility.ToJson(proposal);
            PlayerPrefs.SetString(projectManager.Project.name + "-working-proposal", proposalJSON);
        }

        /// <summary>
        /// Loads all proposals from local storage
        /// </summary>
        public void LoadLocalProsals()
        {
            if (PlayerPrefs.HasKey(projectManager.Project.name + "-proposals"))
            {
                string proposals = PlayerPrefs.GetString(projectManager.Project.name + "-proposals");
                string[] proposalNames = proposals.Split(';');
                foreach (string proposalName in proposalNames)
                {
                    if (proposalName == "") continue;
                    if (PlayerPrefs.HasKey(projectManager.Project.name + "-" + proposalName))
                    {
                        string proposalJSON = PlayerPrefs.GetString(projectManager.Project.name + "-" + proposalName);
                        Proposal proposal = JsonUtility.FromJson<Proposal>(proposalJSON);
                        projectManager.Project.proposals.Add(proposal);
                        Debug.Log($"Loaded proposal {proposal.name} with json {proposalJSON}");
                    }
                    else
                    {
                        Debug.LogError($"Proposal {proposalName} not found in local storage");
                    }
                }
            }
        }

        /// <summary>
        /// Loads the local working proposal if we have one saved, so the user can continue on it
        /// </summary>
        public void LoadLocalWorkingProposal()
        {
            if (PlayerPrefs.HasKey("working-proposal"))
            {
                string proposalJSON = PlayerPrefs.GetString("working-proposal");
                proposal = JsonUtility.FromJson<Proposal>(proposalJSON);
                foreach (ProposalResource resource in proposal.placedObjects)
                {
                    PladdraResource pladdraResource = projectManager.Project.resources.First(r => r.ModelURL == resource.modelId);
                    AddObject(pladdraResource, resource.position, out PlacedObjectController controller);
                    controller.transform.position = resource.position;
                    controller.transform.rotation = Quaternion.Euler(resource.rotation);
                }
            }
        }
        #endregion Proposals

        /// <summary>
        /// Checks if a gameobject is an object in our proposal
        /// </summary>
        /// <param name="gameObject">The gameobject to check</param>
        /// <param name="controller">Returns the controller of the object in case it is an object in our proposal</param>
        /// <returns></returns>
        public bool IsProposalObject(GameObject gameObject, out PlacedObjectController controller)
        {
            bool isPlaced = placedObjects.Any(o => o.gameObject == gameObject);
            controller = isPlaced ? placedObjects.First(o => o.gameObject == gameObject) : null;
            return isPlaced;
        }

        public bool HasActiveProposal()
        {
            return proposal != null;
        }
    }
}