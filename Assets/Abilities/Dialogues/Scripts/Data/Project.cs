using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Pladdra.UX;
using Pladdra.Workspace;
using UnityEngine;
using UnityEngine.Events;
using UntoldGarden.Utils;

namespace Pladdra.DialogueAbility.Data
{
    [Serializable]
    public class Project
    {
        #region Project variables
        public string id;
        public string name;
        public string description;
        public float startScale = 1f;
        public Vector3 startPosition = Vector3.zero;
        public DialogueResource groundPlane;
        public List<DialogueResource> resources;
        public List<Proposal> proposals;
        public Proposal currentProposal;
        public string markerURL;
        public Texture2D marker;
        public bool markerRequired;
        public (double, double) location;
        public Transform origin;
        public Transform projectOrigin;
        public Transform scalePivot;
        public Transform projectContainer;
        public Transform staticResourcesContainer;
        public Transform interactiveResourcesContainer;
        public Transform placedResourcesContainer;
        public bool isLoadedAndInit;
        public bool createdResources;
        #endregion Project variables

        #region Scene References
        WorkspaceController workspaceController;
        public WorkspaceController WorkspaceController { get { return workspaceController; } set { workspaceController = value; } }
        ProposalHandler proposalHandler;
        public ProposalHandler ProposalHandler { get { return proposalHandler; } set { proposalHandler = value; } }
        PivotController pivotController;
        public PivotController PivotController { get { return pivotController; } }
        UXManager uxManager;
        public UXManager UXManager { get { return uxManager; } }
        Transform geoAnchor;
        #endregion Scene References

        #region Events
        public UnityEvent<string, string> OnSaveProposal = new UnityEvent<string, string>();
        #endregion Events

        #region Project handling
        /// <summary>
        /// Initializes the project.
        /// </summary>
        /// <param name="origin">Main projects origin</param>
        /// <param name="uxManager">UXManager</param>
        /// <param name="pivotPrefab">Prefab for scale pivot</param>
        internal void InitProject(Transform origin, UXManager uxManager, GameObject pivotPrefab)
        {
            if (isLoadedAndInit)
            {
                Debug.LogError("Project already initialized");
                return;
            }
            this.origin = origin;
            this.uxManager = uxManager;

            // Create containers
            projectOrigin = new GameObject(name).transform;
            projectOrigin.SetParent(origin);

            //scale pivot container
            scalePivot = GameObject.Instantiate(pivotPrefab).transform;
            scalePivot.SetParent(projectOrigin);
            pivotController = scalePivot.GetComponent<PivotController>();
            pivotController.Init(this);

            //project container
            projectContainer = new GameObject("ProjectContainer").transform;
            projectContainer.SetParent(scalePivot);
            projectContainer.localPosition = Vector3.zero;

            //adding an offset to avoid raycast conflicts between the static resources and the ground plane
            Vector3 raycastOffset = new Vector3(0, 0.01f, 0);
            projectContainer.localPosition += raycastOffset;
            workspaceController = projectContainer.gameObject.AddComponent<WorkspaceController>();
            workspaceController.Init(this);

            proposalHandler = projectOrigin.gameObject.AddComponent<ProposalHandler>(); // Doesn't have to be a MonoBehaviour but keeping it as such in case we need to check debug vars
            // proposalHandler = new ProposalHandler();
            proposalHandler.Init(this);
            proposalHandler.LoadLocalProsals();
            // workspaceController.Scale(startScale);

            // TODO Get user object properly
            projectOrigin.localPosition = Camera.main.gameObject.RelativeToObjectOnGround(new Vector3(0, 0, 4),
                VectorExtensions.RelativeToObjectOptions.OnGroundLayers,
                uxManager.RaycastManager.GetLayerMask("ARMesh"))
                + startPosition;

            isLoadedAndInit = true;
        }

        /// <summary>
        /// Hides the project.
        /// </summary>
        internal void Hide()
        {
            projectOrigin.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the project.
        /// </summary>
        internal void Show()
        {
            projectOrigin.gameObject.SetActive(true);
        }
        #endregion Project

        #region Resources
        /// <summary>
        /// Shows all resources with display rule Static or Interactive
        /// </summary>
        internal void DisplayResources()
        {
            if (createdResources)
            {
                Show();
            }
            else
            {
                CreateStaticAndInteractiveResources();
                if (HasLibraryResources()) CreateLibraryResources();
                if (HasMarkerResources()) CreateMarkerResources();
                createdResources = true;
            }
        }

        /// <summary>
        /// Creates all static and interactive resources, all resources that are displayed when the project is shown.
        /// </summary>
        void CreateStaticAndInteractiveResources()
        {
            List<DialogueResource> resourcesToDisplay = resources.Where(r => r.displayRule == ResourceDisplayRules.Static || r.displayRule == ResourceDisplayRules.Interactive).ToList();

            if (groundPlane != null && groundPlane.gameObject != null)
            {
                groundPlane.gameObject.SetActive(true);
                groundPlane.gameObject.transform.parent = projectContainer;
                groundPlane.gameObject.transform.localPosition = Vector3.zero;

                //TODO Clean up so groundplane is always in static resources
                // select all resources with displayRule auto
                if (resourcesToDisplay == null)
                    resourcesToDisplay = new List<DialogueResource>();
                resourcesToDisplay.Add(groundPlane);
            }
            else if (resourcesToDisplay == null)
            {
                // Add layers so we can place objects on ARMesh 
                uxManager.RaycastManager.AddLayerToLayerMask("allowUserToManipulateSelectedModel", "ARMesh");
                uxManager.RaycastManager.AddLayerToLayerMask("placeableLayers", "ARMesh");

                return;
            }

            // Zero the projectorigin while creating resources to avoid issues with meshCollider generation and other positioning issues
            Vector3 pos = projectOrigin.localPosition;
            projectOrigin.localPosition = Vector3.zero;

            staticResourcesContainer = new GameObject("StaticResources").transform;
            staticResourcesContainer.gameObject.SetAllChildLayers("StaticResources");
            staticResourcesContainer.SetParent(projectContainer);
            staticResourcesContainer.localPosition = Vector3.zero;

            if (HasInteractiveResources())
            {
                interactiveResourcesContainer = new GameObject("InteractiveResources").transform;
                interactiveResourcesContainer.SetParent(projectContainer);
                interactiveResourcesContainer.localPosition = Vector3.zero;
            }

            // Create all static and interactive resources
            foreach (var resource in resourcesToDisplay)
            {
                if (resource.gameObject != null)
                {
                    if (resource.displayRule == ResourceDisplayRules.Static)
                    {
                        resource.gameObject.transform.parent = staticResourcesContainer;
                        resource.gameObject.SetAllChildLayers("StaticResources");
                    }
                    else if (resource.displayRule == ResourceDisplayRules.Interactive)
                    {
                        resource.gameObject.transform.parent = interactiveResourcesContainer;
                        resource.gameObject.AddComponent<InteractiveObjectController>().Init(this, resource);
                    }

                    resource.gameObject.SetActive(true);

                    if (resource.scale != 0) resource.gameObject.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
                    if (resource.position != Vector3.zero) resource.gameObject.transform.localPosition = resource.position;
                    if (resource.rotation != Vector3.zero) resource.gameObject.transform.localRotation = Quaternion.Euler(resource.rotation);
                }
                else
                {
                    Debug.LogWarning($"Project: Could not create static resource {resource.name} because it has no model");
                    // TODO Try redownload model
                }
            }

            // Create collider for static resources
            MeshFilter mf = staticResourcesContainer.gameObject.AddComponent<MeshFilter>();
            mf.mesh = staticResourcesContainer.gameObject.CombineMeshesInChildren();
            staticResourcesContainer.gameObject.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            projectOrigin.localPosition = pos;
        }

        void CreateLibraryResources()
        {
            placedResourcesContainer = new GameObject("PlacedResources").transform;
            placedResourcesContainer.SetParent(projectContainer);
            placedResourcesContainer.localPosition = Vector3.zero;

            // Create thumbnails for all library resources
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;
            foreach (var resource in resources.Where(r => r.displayRule == ResourceDisplayRules.Library))
            {
                if (resource.gameObject != null)
                {
                    resource.thumbnail = RuntimePreviewGenerator.GenerateModelPreview(resource.gameObject.transform, 256, 256);
                }
            }

            // Create GLB safe textures in case we need to export them
            CreateGLBSafeTextures();
        }

        void CreateMarkerResources()
        {
            foreach (var resource in resources.Where(r => r.displayRule == ResourceDisplayRules.Marker))
            {
                // uxManager.ARReferenceImageHandler.AddReferenceImage()
            }
        }

        #endregion Resources

        #region Proposals

        internal void HideProposals()
        {
            proposalHandler.HideAllProposals();
        }

        internal void AddProposal(Proposal proposal)
        {
            if (proposals == null) proposals = new List<Proposal>();
            proposals.Add(proposal);
        }

        internal void DisplayWorkingProposal()
        {
            if (proposals != null && proposals.Contains(proposals.Find(p => p.name == "working-proposal")))
            {
                proposalHandler.LoadLocalWorkingProposal();
            }
            else
            {
                Debug.Log("No working proposal found.");
            }
        }
        #endregion Proposals

        #region Alignment

        /// <summary>
        /// Aligns the project to an AR marker
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        internal void AlignToARMarker(Vector3 position, Quaternion rotation)
        {
            pivotController.MoveWithoutOffset(position);
            var euler = rotation.eulerAngles;
            pivotController.SetRotation(euler.y);
        }

        /// <summary>
        /// Sets the geoanchor of the project.
        /// </summary>
        /// <param name="anchor">Reference to anchor gameObject</param>
        internal void SetGeoAnchor(GameObject anchor)
        {
            geoAnchor = anchor.transform;
            UnityEngine.Object.FindObjectOfType<Pladdra.ARDebug.PlaneVisualiser>().AddARObject(geoAnchor.gameObject); //Temporary solution for AR viz
            AlignToGeoAnchor();
        }

        /// <summary>
        /// Alings the project to the geo anchor.
        /// </summary>
        internal void AlignToGeoAnchor()
        {
            Vector3 pos = geoAnchor.position;
            pos.y = uxManager.AppManager.ARSessionManager.GetDefaultPlaneY(); // To make sure the model is placed on the ground, as the geoanchor quite often is a bit off
            pivotController.MoveWithoutOffset(pos);
            var euler = geoAnchor.rotation.eulerAngles;
            pivotController.SetRotation(euler.y);
        }
        #endregion Alignment

        /// <summary>
        /// Creates 1x1 textures for all materials that have no texture.
        /// This is a solution for glb export since color-only materials are not exported.
        /// </summary>
        private void CreateGLBSafeTextures()
        {
            foreach (var resource in resources)
            {
                if (resource.gameObject == null)
                {
                    Debug.LogError($"Project: Could not create texture for {resource.name} because it has no gameobject");
                    continue;
                }
                Material[] materials = resource.gameObject.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.sharedMaterials).ToArray();
                foreach (Material material in materials)
                {
                    if (material.mainTexture == null)
                        material.ColorToTexture();
                }
            }
        }

        #region Booleans
        internal bool HasProposals()
        {
            return proposals != null && proposals.Count > 0;
        }

        internal bool HasLibraryResources()
        {
            return resources.Where(r => r.displayRule == ResourceDisplayRules.Library).Count() > 0;
        }

        internal bool HasInteractiveResources()
        {
            return resources.Where(r => r.displayRule == ResourceDisplayRules.Interactive).Count() > 0;
        }

        internal bool HasMarkerResources()
        {
            return resources.Where(r => r.displayRule == ResourceDisplayRules.Marker).Count() > 0;
        }
        #endregion Booleans
    }
}