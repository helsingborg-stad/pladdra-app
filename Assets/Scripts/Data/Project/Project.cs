using System;
using System.Collections.Generic;
using Abilities;
using Pladdra.Workspace;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Pladdra.Data
{
    public class Project
    {
        public string Id;
        // public DialogPlane Plane;
        public string Name;
        public bool allowAnyUserToInteract;
        public List<PladdraResource> Resources; // That users can ineract with
        public List<PladdraResource> StaticResources; // That users can't ineract with
        public List<UserProposal> UserProposals;
        public ProjectARMarker Marker;
        public bool markerRequired;
        public Transform projectOrigin;

        internal void InstantiateStaticResources(Transform origin)
        {
            foreach (var resource in StaticResources)
            {
                if(resource.Model != null)
                {
                    var model = GameObject.Instantiate(resource.Model, origin);
                    model.transform.localPosition = resource.Position;
                    model.transform.localRotation = Quaternion.Euler(resource.Rotation);
                }
                else
                {
                    Debug.LogWarning($"Project: Could not instantiate static resource {resource.Name} because it has no model");
                }
            }
        }

        internal void HideUserProposals()
        {
            throw new NotImplementedException();
        }

        internal void ShowUserProposal()
        {
            throw new NotImplementedException();
        }

        internal bool UserCanInteract()
        {
            return allowAnyUserToInteract && Resources.Count > 0;
        }

        internal void ShowInteractiveResources(WorkspaceManager workspaceManager, IAbility ability, WorkspaceConfiguration configuration, Transform origin)
        {
            configuration.Origin.go = configuration.Origin.go ?? origin.gameObject;
            workspaceManager.Activate(ability, configuration); // TODO Change so it's referencing the project object, not the ability and the config. This should happ
        }

        internal void HideInteractiveResources(WorkspaceManager workspaceManager)
        {
            workspaceManager.Deactivate(); 
        }
    }
}
