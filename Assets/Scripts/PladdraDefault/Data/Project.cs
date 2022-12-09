using System;
using System.Collections.Generic;
using Abilities;
using Pladdra.Workspace;
using UnityEngine;
using UntoldGarden.Utils;

namespace Pladdra.DefaultAbility.Data
{
    [Serializable]
    public class Project
    {
        public string id;
        public string name;
        public string description;
        public float startScale = 1f;
        public Vector3 startPosition = Vector3.zero;
        public bool allowAnyUserToInteract;
        public PladdraResource groundPlane;
        public List<PladdraResource> resources; // That users can ineract with
        public List<PladdraResource> staticResources; // That users can't ineract with
        public List<Proposal> proposals;
        public Proposal currentProposal;
        public string markerURL;
        public Texture2D marker;
        public bool markerRequired;
        public Transform origin;
        public Transform projectOrigin;
        public Transform projectContainer;
        public Transform staticResourcesOrigin;
        WorkspaceController workspaceController;
        public WorkspaceController WorkspaceController { get { return workspaceController; } set { workspaceController = value; } }

        public bool isLoaded;

        internal void InitProject(Transform origin, ProposalManager proposalManager, UXManager uxManager)
        {
            this.origin = origin;

            // Create containers
            projectOrigin = new GameObject(name).transform;
            projectOrigin.SetParent(origin);

            projectContainer = new GameObject("ProjectContainer").transform;
            projectContainer.SetParent(projectOrigin);
            projectContainer.localPosition = Vector3.zero;
            workspaceController = projectContainer.gameObject.AddComponent<WorkspaceController>();
            workspaceController.Init(proposalManager, uxManager);

            CreateThumbnails();

            isLoaded = true;
        }

        internal void ShowStaticResources()
        {
            // TODO This can be cleaned up
            if (staticResourcesOrigin != null)
            {
                projectOrigin.gameObject.SetActive(true);
                return;
            }

            staticResourcesOrigin = new GameObject("StaticResources").transform;
            staticResourcesOrigin.SetParent(projectContainer);
            staticResourcesOrigin.localPosition = Vector3.zero;

            // Create all static resources
            foreach (var resource in staticResources)
            {
                if (resource.model != null)
                {
                    var model = GameObject.Instantiate(resource.model, staticResourcesOrigin);
                    model.SetActive(true);
                    model.layer = 0;
                    if (resource.scale != 0) model.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
                    if (resource.position != Vector3.zero) model.transform.localPosition = resource.position;
                    if (resource.rotation != Vector3.zero) model.transform.localRotation = Quaternion.Euler(resource.rotation);
                }
                else
                {
                    Debug.LogWarning($"Project: Could not instantiate static resource {resource.name} because it has no model");
                    // TODO Try redownload model
                }
            }
            staticResourcesOrigin.gameObject.SetAllChildLayers("StaticResources");
            //TODO Set start scale

            // Create collider
            MeshFilter mf = staticResourcesOrigin.gameObject.AddComponent<MeshFilter>();
            mf.mesh = staticResourcesOrigin.gameObject.CombineMeshesInChildren();
            staticResourcesOrigin.gameObject.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            // Set Scale and position
            // TODO Get user object properly
            projectOrigin.localPosition = Camera.main.gameObject.RelativeToObject(new Vector3(0, 0, 4),
                VectorExtensions.RelativeToObjectOptions.OnGroundLayers,
                new string[] { "ARMesh" })
                + startPosition;
            if (startScale != 1) workspaceController.Scale(startScale);
        }

        internal void Hide()
        {
            projectOrigin.gameObject.SetActive(false);
        }

        internal void Show()
        {
            projectOrigin.gameObject.SetActive(true);
        }

        internal bool UserCanInteract()
        {
            return allowAnyUserToInteract && resources.Count > 0;
        }

        internal bool HasProposals()
        {
            return proposals != null && proposals.Count > 0;
        }

        internal void AddProposal(Proposal proposal)
        {
            if (proposals == null) proposals = new List<Proposal>();
            proposals.Add(proposal);
        }

        internal void CreateThumbnails()
        {
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            foreach (var resource in resources)
            {
                if (resource.model != null)
                {
                    resource.thumbnail = RuntimePreviewGenerator.GenerateModelPreview(resource.model.transform, 256, 256);
                }
            }
        }


    }
}
