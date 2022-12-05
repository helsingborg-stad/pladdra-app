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
        public List<PladdraResource> resources; // That users can ineract with
        public List<PladdraResource> staticResources; // That users can't ineract with
        public List<Proposal> proposals;
        public Proposal currentProposal;
        public ARMarker marker;
        public bool markerRequired;
        public Transform origin;
        public Transform projectOrigin;
        public Transform projectContainer;
        public Transform staticResourcesOrigin;
        public WorkspaceController workspaceController;

        public bool isLoaded;

        internal void InitProject(Transform origin, ProposalManager proposalManager, InteractionManager interactionManager)
        {
            this.origin = origin;

            // Create containers
            projectOrigin = new GameObject(name).transform;
            projectOrigin.SetParent(origin);

            projectContainer = new GameObject("ProjectContainer").transform;
            projectContainer.SetParent(projectOrigin);
            projectContainer.localPosition = Vector3.zero;
            workspaceController = projectContainer.gameObject.AddComponent<WorkspaceController>();
            workspaceController.Init(proposalManager, interactionManager);
        }
        internal void InstantiateStaticResources()
        {
            staticResourcesOrigin = new GameObject("StaticResources").transform;
            staticResourcesOrigin.SetParent(projectContainer);
            staticResourcesOrigin.localPosition = Vector3.zero;

            // Create all static resources
            foreach (var resource in staticResources)
            {
                if (resource.Model != null)
                {
                    var model = GameObject.Instantiate(resource.Model, staticResourcesOrigin);
                    model.SetActive(true);
                    model.layer = 0;
                    if (resource.Scale != Vector3.zero) model.transform.localScale = resource.Scale;
                    if (resource.Position != Vector3.zero) model.transform.localPosition = resource.Position;
                    if (resource.Rotation != Vector3.zero) model.transform.localRotation = Quaternion.Euler(resource.Rotation);
                }
                else
                {
                    Debug.LogWarning($"Project: Could not instantiate static resource {resource.Name} because it has no model");
                    // TODO Try redownload model
                }
            }

            //TODO Set start scale

            // Create collider
            MeshFilter mf = staticResourcesOrigin.gameObject.AddComponent<MeshFilter>();
            mf.mesh = staticResourcesOrigin.gameObject.CombineMeshesInChildren();
            staticResourcesOrigin.gameObject.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            // Set Scale and position
            // TODO Get user object
            // TODO Wait for AR plane
            projectOrigin.localPosition = Camera.main.gameObject.RelativeToObject(new Vector3(0, 0, 4),
                VectorExtensions.RelativeToObjectOptions.OnGroundLayers,
                new string[] { "ARMesh" })
                + startPosition;
            if (startScale != 1) workspaceController.Scale(startScale);

            foreach (PladdraResource resource in resources)
            {
                workspaceController.SetThumbnail(resource.Thumbnail);
            }
        }

        internal void Hide()
        {
            foreach (var resource in staticResources)
            {
                if (resource.Model != null)
                {
                    resource.Model.SetActive(false);
                }
            }
        }

        internal bool UserCanInteract()
        {
            return allowAnyUserToInteract && resources.Count > 0;
        }

        internal bool HasProposals()
        {
            return proposals != null && proposals.Count > 0;
        }

        internal void CreateThumbnails()
        {
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            foreach (var resource in resources)
            {
                if (resource.Model != null)
                {
                    resource.Thumbnail = RuntimePreviewGenerator.GenerateModelPreview(resource.Model.transform, 256, 256);
                }
            }
        }
    }
}
