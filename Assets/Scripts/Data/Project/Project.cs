using System;
using System.Collections.Generic;
using Abilities;
using Pladdra.Workspace;
using UnityEngine;

namespace Pladdra.Data
{
    [Serializable]
    public class Project
    {
        // TODO decapitalize
        public string Id;
        // public DialogPlane Plane;
        public string Name;
        public string Description;
        public bool allowAnyUserToInteract;
        public List<PladdraResource> Resources; // That users can ineract with
        public List<PladdraResource> StaticResources; // That users can't ineract with
        public List<UserProposal> UserProposals;
        public UserProposal CurrentUserProposal;
        public ARMarker Marker;
        public bool markerRequired;
        public Transform projectOrigin;


        internal void InstantiateStaticResources(Transform origin)
        {
            foreach (var resource in StaticResources)
            {
                if (resource.Model != null)
                {
                    var model = GameObject.Instantiate(resource.Model, origin);
                    model.SetActive(true);
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
        }

        internal void Hide()
        {
            foreach (var resource in StaticResources)
            {
                if (resource.Model != null)
                {
                    resource.Model.SetActive(false);
                }
            }
            HideUserProposals();
            // TODO Hide CurrentUserProposal
        }

        internal void HideUserProposals()
        {
            // TODO
        }

        internal void ShowUserProposal()
        {
            // TODO
        }

        internal bool UserCanInteract()
        {
            return allowAnyUserToInteract && Resources.Count > 0;
        }

        internal bool HasProposals()
        {
            return UserProposals != null && UserProposals.Count > 0;
        }

        internal void CreateThumbnails()
        {
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            foreach (var resource in Resources)
            {
                resource.Thumbnail = RuntimePreviewGenerator.GenerateModelPreview(resource.Model.transform, 512, 512);
            }
        }
    }
}
