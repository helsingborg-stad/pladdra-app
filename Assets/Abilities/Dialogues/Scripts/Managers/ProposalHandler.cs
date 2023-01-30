using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.DialogueAbility.Data;
using System.Linq;
using System;
using UntoldGarden.Utils;

namespace Pladdra.DialogueAbility
{
    [DisallowMultipleComponent]
    public class ProposalHandler : MonoBehaviour
    {
        #region Private
        Project project;
        [GrayOut][SerializeField] Proposal proposal;
        public Proposal Proposal { get => proposal; }
        [GrayOut] public List<PlacedObjectController> placedObjects = new List<PlacedObjectController>();
        List<GameObject> loadedProposalObjects = new List<GameObject>();
        bool hasAddedSaveProposalMenuItem;
        #endregion Private

        public void Init(Project project)
        {
            this.project = project;
        }

        #region Proposal Objects

        /// <summary>
        /// Adds a new object to our proposal
        /// </summary>
        /// <param name="resource">The resource containing the object</param>
        /// <param name="position">The position of the object</param>
        /// <param name="controller">The controller of the object</param>
        public void AddObject(DialogueResource resource, Vector3 position, out PlacedObjectController controller, bool save = true)
        {
            Debug.Log($"Adding object {resource.name}, placed object count {placedObjects.Count}");

            GameObject obj = GameObject.Instantiate(project.UXManager.settings.objectPrefab, project.placedResourcesContainer);
            obj.transform.SetParent(project.placedResourcesContainer);
            obj.name = resource.name;
            obj.transform.position = position;

            GameObject model = GameObject.Instantiate(resource.gameObject, obj.transform);
            if (resource.scale != 0) model.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
            controller = obj.GetComponent<PlacedObjectController>() ?? obj.AddComponent<PlacedObjectController>();
            controller.Init(project, resource);
            placedObjects.Add(controller);
            model.SetActive(true);

            // Create a new proposal 
            if (proposal == null)
            {
                Debug.Log("ProposalManager: Creating new proposal");
                proposal = new Proposal();
                proposal.name = "working-proposal";
                proposal.placedObjects = new List<ProposalResource>();
            }

            // Adds a menu item for saving the proposal
            if (!hasAddedSaveProposalMenuItem)
            {
                AddSaveProposalMenuItem();
            }

            proposal.placedObjects.Add(new ProposalResource()
            {
                modelId = resource.url,
                localId = controller.Id,
                name = resource.name,
                position = obj.transform.localPosition,
                rotation = Vector3.zero,
            });
            if (save)
                SaveWorkingProposalLocally();
        }

        /// <summary>
        /// Adds an object to a proposal when loaded.
        /// </summary>
        /// <param name="resource">The resource containing the object</param>
        /// <param name="position">The position of the object</param>
        /// <param name="rotation">The rotation of the object</param>
        public void AddObject(ProposalResource resource, Vector3 position, Vector3 rotation)
        {
            Debug.Log($"Adding object {resource.name} at {position} with rotation {rotation}");
            GameObject modelContainer = new GameObject(resource.name);
            modelContainer.transform.SetParent(project.placedResourcesContainer);

            GameObject obj = project.resources.Find(x => x.url == resource.modelId).gameObject;
            GameObject model = GameObject.Instantiate(obj, modelContainer.transform);
            model.SetActive(true);

            modelContainer.transform.localPosition = position;
            modelContainer.transform.rotation = Quaternion.Euler(rotation);
            loadedProposalObjects.Add(modelContainer);
        }

        /// <summary>
        /// Removes an object from our proposal
        /// </summary>
        /// <param name="PlacedObjectController">The controller of the object to remove</param>
        public void RemoveObject(PlacedObjectController placedObjectController)
        {
            Debug.Log($"Removing object {placedObjectController.resource.name}");
            placedObjects.Remove(placedObjectController);
            proposal.placedObjects.Remove(proposal.placedObjects.Find(x => x.localId == placedObjectController.Id));
            SaveWorkingProposalLocally();
        }

        /// <summary>
        /// Removes all objects from the proposal
        /// </summary>
        public void RemoveAllObjects()
        {
            foreach (PlacedObjectController placedObject in placedObjects)
            {
                GameObject.Destroy(placedObject.gameObject);
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
                HideProposal();
                NewProposal();
                //TODO clean this up
            }
            this.proposal = project.proposals.Find(p => p.name == proposal);
            float scale = project.WorkspaceController.CurrentScale;
            project.WorkspaceController.Scale(1);
            foreach (ProposalResource resource in this.proposal.placedObjects)
            {
                AddObject(resource, resource.position, resource.rotation);
            }
            project.WorkspaceController.Scale(scale);
        }

        /// <summary>
        /// Hides a displayed proposal
        /// </summary>
        public void HideProposal()
        {
            foreach (GameObject obj in loadedProposalObjects)
            {
                GameObject.Destroy(obj);
            }
            loadedProposalObjects.Clear();
            proposal = null;
        }

        public void HideAllProposals()
        {
            //TODO
            HideProposal();
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
            Debug.Log($"Saving proposal {name}");
            if (proposal == null) return;
            proposal.name = name;
            project.AddProposal(proposal);
            string proposalJSON = JsonUtility.ToJson(proposal);
            project.OnSaveProposal.Invoke(name, proposalJSON);
            SaveProposalLocally(name, proposalJSON);
            PlayerPrefs.DeleteKey(project.name + "-working-proposal"); // Remove the working proposal as the user has saved it
        }

        /// <summary>
        /// Saves proposal locally so it's easily available for the next session
        /// Overwrites any previously saved proposal with the same name
        /// </summary>
        public void SaveProposalLocally(string name, string json)
        {
            string proposals = "";
            if (PlayerPrefs.HasKey(project.name + "-proposals"))
            {
                proposals = PlayerPrefs.GetString(project.name + "-proposals");
            }
            string[] proposalsArray = proposals.Split(';');
            if (!proposalsArray.Contains(name))
                proposals += name + ";";

            PlayerPrefs.SetString(project.name + "-proposals", proposals);
            PlayerPrefs.SetString(project.name + "-" + name, json);
        }

        /// <summary>
        /// Invoked on every model-related action
        /// </summary>
        public void SaveWorkingProposalLocally()
        {
            if (proposal == null) return;
            string proposalJSON = JsonUtility.ToJson(proposal);
            Debug.Log($"Saving working proposal {proposalJSON}");
            SaveProposalLocally("working-proposal", proposalJSON);
        }

        /// <summary>
        /// Loads all proposals from local storage
        /// </summary>
        public void LoadLocalProsals()
        {
            if (PlayerPrefs.HasKey(project.name + "-proposals"))
            {
                string proposals = PlayerPrefs.GetString(project.name + "-proposals");
                Debug.Log($"Loading proposals {proposals}");
                string[] proposalNames = proposals.Split(';');
                foreach (string proposalName in proposalNames)
                {
                    if (proposalName == "") continue;
                    if (PlayerPrefs.HasKey(project.name + "-" + proposalName))
                    {
                        string proposalJSON = PlayerPrefs.GetString(project.name + "-" + proposalName);
                        Proposal proposal = JsonUtility.FromJson<Proposal>(proposalJSON);
                        project.AddProposal(proposal);
                        Debug.Log($"Loaded proposal {proposal.name} with json {proposalJSON}");
                    }
                    else
                    {
                        Debug.Log($"Proposal {proposalName} not found in local storage");
                    }
                }
            }
        }

        /// <summary>
        /// Loads the local working proposal so the user can continue on it
        /// </summary>
        public void LoadLocalWorkingProposal()
        {
            if (project != null && project.proposals != null && project.proposals.Contains(project.proposals.Find(p => p.name == "working-proposal")))
            {
                Debug.Log("Loading working proposal");
                this.proposal = project.proposals.Find(p => p.name == "working-proposal");
                ProposalResource[] resources = this.proposal.placedObjects.ToArray();
                proposal.placedObjects.Clear(); // We clear the list and readd them, easier that way as we otherwise need to have two flows for adding objects

                foreach (ProposalResource resource in resources)
                {
                    DialogueResource DialogueResource = project.resources.Find(x => x.url == resource.modelId);
                    AddObject(DialogueResource, resource.position.MakeGlobal(project.projectContainer), out PlacedObjectController controller, false);
                    controller.SetRotation(resource.rotation.y);
                    controller.StopAllCoroutines(); //To avoid updating the proposal after placing initial resources.
                }
            }
            else
            {
                Debug.Log("No working proposal found");
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

        /// <summary>
        /// Adds a new menu item for saving a proposal.
        /// </summary>
        void AddSaveProposalMenuItem()
        {
            project.UXManager.UIManager.MenuManager.AddMenuItem(new Pladdra.UI.MenuItem()
            {
                id = "save-proposal",
                name = "Spara förslag", // TODO Change to call from UItexts
                action = () =>
                {
                    Pladdra.UX.UXHandler ux = new Pladdra.DialogueAbility.UX.AllowUserToSaveProposal(project.UXManager);
                    project.UXManager.UseUxHandler(ux);
                }
            });

            hasAddedSaveProposalMenuItem = true;
        }
    }
}