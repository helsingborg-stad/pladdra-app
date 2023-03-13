using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UntoldGarden.Utils;
using GLTFast;
using System.Threading.Tasks;
using Pladdra.ARSandbox.Dialogues.UX;
using System.IO;
using UnityEngine.XR.ARFoundation;
using Pladdra.UI;

namespace Pladdra.ARSandbox.Dialogues.Data
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
        public List<DialogueResource> resources;
        public List<Proposal> proposals;
        public Proposal currentProposal;
        public (string url, bool required, float width, Texture2D image) marker;
        public (double lat, double lon, float rotation) location;
        #endregion Project variables

        #region Project containers
        public Transform origin;
        public Transform projectOrigin;
        public Transform scalePivot;
        public Transform projectContainer;
        public Transform staticResourcesContainer;
        public Transform interactiveResourcesContainer;
        public Transform placedResourcesContainer;
        public Transform markerResourcesContainer;
        #endregion Project containers

        #region Project state
        // internal bool isLoadedAndInit;
        internal bool isCreated;
        internal bool overrideGeolocation;
        internal bool hasProposals { get { return proposals != null && proposals.Count > 0; } }
        internal bool hasLibraryResources { get { return resources.Where(r => r.displayRule == ResourceDisplayRules.Library).Count() > 0; } }
        internal bool hasInteractiveResources { get { return resources.Where(r => r.displayRule == ResourceDisplayRules.Interactive).Count() > 0; } }
        internal bool hasMarkerResources { get { return resources.Where(r => r.displayRule == ResourceDisplayRules.Marker).Count() > 0; } }
        internal bool requiresGeolocation { get { return location.lat != 0 && location.lon != 0 && !overrideGeolocation; } }
        internal bool hasTrackerMarker { get { return trackedImage != null; } }
        #endregion Project state

        #region Scene References
        WorkspaceController workspaceController;
        public WorkspaceController WorkspaceController { get { return workspaceController; } set { workspaceController = value; } }
        ProposalHandler proposalHandler;
        public ProposalHandler ProposalHandler { get { return proposalHandler; } set { proposalHandler = value; } }
        PivotController pivotController;
        public PivotController PivotController { get { return pivotController; } }
        DialoguesUXManager uxManager;
        public DialoguesUXManager UXManager { get { return uxManager; } }
        ProjectManager projectManager;
        public ProjectManager ProjectManager { get { return projectManager; } }
        Transform geoAnchor;
        #endregion Scene References

        #region Events
        public UnityEvent<string, string> OnSaveProposal = new UnityEvent<string, string>();
        #endregion Events

        #region Private
        bool createdProjectContainers;
        bool hasAddedMarkerListener = false;
        bool addedAlignToARMenuItem = false;
        ARTrackedImage trackedImage;
        MeshCollider staticResourcesMeshCollider;
        public MeshCollider StaticResourcesMeshCollider { get { return staticResourcesMeshCollider; } }
        #endregion Privateß


        #region Project handling
        /// <summary>
        /// Initializes the project.
        /// </summary>
        /// <param name="projectManager">ProjectManager</param>
        /// <param name="uxManager">UXManager</param>
        /// <param name="pivotPrefab">Prefab for scale pivot</param>
        internal void Init(ProjectManager projectManager, DialoguesUXManager uxManager)
        {
            this.projectManager = projectManager;
            this.origin = projectManager.Origin();
            this.uxManager = uxManager;
        }

        internal async Task CreateProject()
        {
            if (isCreated)
            {
                Show();
            }
            else
            {
                if (!createdProjectContainers) CreateProjectContainers();
                if (!marker.required) PlaceProject();
                if (!marker.url.IsNullOrEmptyOrFalse() && marker.image == null) await CreateProjectMarker();
                await CreateStaticAndInteractiveResources();
                if (hasLibraryResources) await CreateLibraryResources();
                if (hasMarkerResources) await CreateMarkerResources();
                LoadProposals();
                DisplayWorkingProposal();

                isCreated = true;
            }
        }

        internal void CreateProjectContainers()
        {
            // Create containers
            projectOrigin = new GameObject(name).transform;
            projectOrigin.SetParent(origin);

            if (geoAnchor != null)
            {
                AlignToGeoAnchor();
            }

            //scale pivot container
            scalePivot = GameObject.Instantiate(uxManager.settings.pivotPrefab).transform;
            scalePivot.SetParent(projectOrigin);
            scalePivot.localPosition = Vector3.zero;

            pivotController = scalePivot.GetComponent<PivotController>();
            pivotController.Init(this);

            //project container
            projectContainer = new GameObject("ProjectContainer").transform;
            projectContainer.SetParent(scalePivot);
            projectContainer.localPosition = Vector3.zero;

            //adding an offset to avoid raycast conflicts between the static resources and the ground plane
            Vector3 raycastOffset = new Vector3(0, 0.01f, 0);
            projectContainer.localPosition += raycastOffset;
            workspaceController = projectContainer.gameObject.AddComponent<WorkspaceController>(); //TODO Don't add if not needed
            workspaceController.Init(this);

            createdProjectContainers = true;
        }
        void PlaceProject()
        {
            if (!requiresGeolocation)
            {
                projectOrigin.localPosition = uxManager.AppManager.ARSessionManager.GetUser().gameObject.RelativeToObjectOnGround(new Vector3(0, 0, 4),
                    VectorExtensions.RelativeToObjectOptions.OnGroundLayers,
                    uxManager.RaycastManager.GetLayerMask("ARMesh"))
                    + startPosition;
            }
            else
            {
                AlignToGeoAnchor();
            }
        }


        /// <summary>
        /// Hides the project.
        /// </summary>
        internal void Hide()
        {
            if (projectOrigin != null)
                projectOrigin.gameObject.SetActive(false);
            else
                Debug.Log("Project origin is null in project " + name);
        }

        /// <summary>
        /// Shows the project.
        /// </summary>
        internal void Show()
        {
            if (projectOrigin != null)
                projectOrigin.gameObject.SetActive(true);
            else
                Debug.Log("Project origin is null in project " + name);
        }
        #endregion Project

        #region Resources


        /// <summary>
        /// Creates all static and interactive resources, all resources that are displayed when the project is shown.
        /// </summary>
        async Task CreateStaticAndInteractiveResources()
        {

            List<DialogueResource> resourcesToDisplay = resources.Where(r => r.displayRule == ResourceDisplayRules.Static || r.displayRule == ResourceDisplayRules.Interactive).ToList();
            Debug.Log("CreateStaticAndInteractiveResources, count " + resourcesToDisplay.Count);

            DialogueResource groundPlane = resourcesToDisplay.FirstOrDefault(r => r.name == "GroundPlane");
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
            else if (resourcesToDisplay == null || resourcesToDisplay.Count == 0)
            {
                // Add layers so we can place objects on ARMesh 
                uxManager.RaycastManager.AddLayerToLayerMask("allowUserToManipulateSelectedModel", "ARMesh");
                uxManager.RaycastManager.AddLayerToLayerMask("placeableLayers", "ARMesh");

                return;
            }

            // Zero the projectorigin while creating resources to avoid issues with staticResourcesMeshCollider generation and other positioning issues
            Vector3 pos = projectOrigin.localPosition;
            projectOrigin.localPosition = Vector3.zero;
            Vector3 rot = projectOrigin.localRotation.eulerAngles;
            projectOrigin.localRotation = Quaternion.Euler(Vector3.zero);

            staticResourcesContainer = new GameObject("StaticResources").transform;
            staticResourcesContainer.gameObject.SetAllChildLayers("StaticResources");
            staticResourcesContainer.SetParent(projectContainer);
            staticResourcesContainer.localPosition = Vector3.zero;
            staticResourcesContainer.localRotation = Quaternion.identity;

            if (hasInteractiveResources)
            {
                interactiveResourcesContainer = new GameObject("InteractiveResources").transform;
                interactiveResourcesContainer.SetParent(projectContainer);
                interactiveResourcesContainer.localPosition = Vector3.zero;
                interactiveResourcesContainer.localRotation = Quaternion.identity;
            }

            // Create all static and interactive resources
            foreach (var resource in resourcesToDisplay)
            {
                resource.gameObject = new GameObject(resource.name);
                resource.gameObject.AddComponent<GltfAsset>().Url = "file://" + resource.path;

                Debug.Log($"Project: Creating static resource {resource.name} with display rule {resource.displayRule}");

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

                if (resource.scale != 0) resource.gameObject.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
                resource.gameObject.transform.localPosition = resource.position;
                resource.gameObject.transform.localRotation = Quaternion.Euler(resource.rotation);
            }

            while (staticResourcesContainer.gameObject.GetComponentsInChildren<GltfAsset>().Any(g => g.SceneInstance == null))
            {
                await Task.Yield();
            }

            // Create collider for static resources
            MeshFilter mf = staticResourcesContainer.gameObject.AddComponent<MeshFilter>();
            Mesh mesh = staticResourcesContainer.gameObject.CombineMeshesInChildren();
            if (mesh != null)
            {
                mf.mesh = mesh;
                staticResourcesMeshCollider = staticResourcesContainer.gameObject.AddComponent<MeshCollider>();
                staticResourcesMeshCollider.sharedMesh = mf.mesh;
            }
            else
            {
                Debug.Log("No mesh found for static resources");
            }

            // Reset projectOrigin to original position and rotation
            projectOrigin.localPosition = pos;
            projectOrigin.localRotation = Quaternion.Euler(rot);
        }

        /// <summary>
        /// Creates a container for library resources placed by the user.
        /// Creates thumbnails for all library resources. This is a bit time consuming, but the only alternative would be to store previews on wordpress.
        /// </summary>
        /// <returns></returns>
        async Task CreateLibraryResources()
        {
            placedResourcesContainer = new GameObject("PlacedResources").transform;
            placedResourcesContainer.SetParent(projectContainer);
            placedResourcesContainer.localPosition = new Vector3(0, 0, 0);

            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.MarkTextureNonReadable = false;

            string imagesPath = Application.persistentDataPath + "/Images";
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            foreach (var resource in resources.Where(r => r.displayRule == ResourceDisplayRules.Library))
            {
                Debug.Log("Creating library resource for " + resource.name);
                try
                {
                    string thumbnailPath = Path.Combine(imagesPath, resource.name + ".png");

                    if (File.Exists(thumbnailPath))
                    {
                        byte[] bytes = File.ReadAllBytes(thumbnailPath);
                        resource.thumbnail = new Texture2D(256, 256);
                        resource.thumbnail.LoadImage(bytes);
                    }
                    else
                    {
                        GameObject go = new GameObject();
                        go.transform.position = new Vector3(0, 100, 0);
                        go.AddComponent<GltfAsset>().Url = "file://" + resource.path;
                        while (go.GetComponent<GltfAsset>().SceneInstance == null)
                        {
                            await Task.Yield();
                        }
                        resource.thumbnail = RuntimePreviewGenerator.GenerateModelPreview(go.transform, 256, 256);
                        File.WriteAllBytes(thumbnailPath, resource.thumbnail.EncodeToPNG());
                        UnityEngine.Object.Destroy(go);

                        await Task.Yield();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error while loading library resource: " + e);
                }
            }
        }

        #endregion Resources

        #region Markers

        internal async Task CreateProjectMarker()
        {
            projectManager.WebRequestHandler.StartCoroutine(projectManager.WebRequestHandler.LoadImage(marker.url, (Result result, string errors, Texture2D image) =>
                {
                    if (result == Result.Success)
                        marker.image = image;
                    else
                        uxManager.UIManager.ShowError($"Error loading marker image for project {name}, error: {errors}");
                }));

            bool debug = true;

            while (marker.image == null && Application.isPlaying)
            {
                if (debug)
                {
                    Debug.Log($"Waiting for project {name} marker image to load");
                    debug = false;
                }
                await Task.Yield();
            }

            uxManager.ARReferenceImageHandler.AddReferenceImage(marker.image, name, marker.width);

            CreateMarkerListener();
        }

        async Task CreateMarkerResources()
        {
            markerResourcesContainer = new GameObject("MarkerResources").transform;
            markerResourcesContainer.SetParent(projectContainer);
            markerResourcesContainer.localPosition = new Vector3(0, 0, 0);

            foreach (var resource in resources.Where(r => r.displayRule == ResourceDisplayRules.Marker))
            {
                Debug.Log($"Project: Creating marker resource {resource.name} with url {resource.marker.url}");
                projectManager.WebRequestHandler.StartCoroutine(projectManager.WebRequestHandler.LoadImage(resource.marker.url, (Result result, string errors, Texture2D image) =>
                {
                    if (result == Result.Success)
                        resource.marker.image = image;
                    else
                        uxManager.UIManager.ShowError($"Error loading marker image for resource {resource.name}, error: {errors}");
                }));

                bool debug = true;

                while (resource.marker.image == null && Application.isPlaying)
                {
                    if (debug)
                    {
                        Debug.Log($"Waiting for resource {resource.name} marker image to load");
                        debug = false;
                    }
                    await Task.Yield();
                }

                uxManager.ARReferenceImageHandler.AddReferenceImage(resource.marker.image, resource.name, resource.marker.width);
            }

            CreateMarkerListener();
        }

        void CreateMarkerListener()
        {
            if (!hasAddedMarkerListener)
            {
                uxManager.ARReferenceImageHandler.OnImageTracked.AddListener(OnMarkerFound);
                hasAddedMarkerListener = true;
            }
        }

        void OnMarkerFound(ARTrackedImage trackedImage)
        {
            Debug.Log($"Found marker {trackedImage.referenceImage.name}");
            if (trackedImage.referenceImage.name == this.name)
            {
                Debug.Log($"Found project {name} marker");
                this.trackedImage = trackedImage;
                AlignToARMarker();
                if (!addedAlignToARMenuItem)
                {
                    uxManager.UIManager.MenuManager.AddMenuItem(new MenuItem()
                    {
                        id = "alignToARMarker",
                        name = "Placera på markör",
                        action = () =>
                        {
                            AlignToARMarker();
                        }
                    });
                    addedAlignToARMenuItem = true;
                }
            }
            else
            {
                DialogueResource resource = resources.FirstOrDefault(r => r.name == trackedImage.referenceImage.name);
                Debug.Log($"Found resource {trackedImage.referenceImage.name} marker");
                if (resource != null)
                {
                    resource.gameObject = new GameObject(resource.name);
                    resource.gameObject.AddComponent<GltfAsset>().Url = "file://" + resource.path;
                    resource.gameObject.transform.SetParent(markerResourcesContainer);
                    if (resource.scale != 0) resource.gameObject.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
                    resource.gameObject.AddComponent<MarkerObjectController>().Init(resource, trackedImage);
                }
                else
                {
                    Debug.Log($"No resource found for marker {trackedImage.referenceImage.name}");
                }
            }
        }


        #endregion Markers

        #region Proposals

        internal void LoadProposals()
        {
            proposalHandler = projectContainer.gameObject.AddComponent<ProposalHandler>(); // Doesn't have to be a MonoBehaviour but keeping it as such in case we need to check debug vars
            proposalHandler.Init(this);
            proposalHandler.LoadLocalProsals();
        }

        internal void HideProposals()
        {
            if (proposalHandler != null)
                proposalHandler.HideAllProposals();
            else
                Debug.Log("No proposal handler found in project " + name);
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
        internal void AlignToARMarker()
        {
            pivotController.MoveWithoutOffset(trackedImage.transform.position);
            var euler = trackedImage.transform.rotation.eulerAngles;
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
        }

        /// <summary>
        /// Realigns the project to the geo anchor.
        /// </summary>
        internal void AlignToGeoAnchor()
        {
            if (projectOrigin == null || geoAnchor == null)
                return;

            if (Vector3.Distance(geoAnchor.position, projectOrigin.transform.position) > 0.1f)
            {
                Debug.Log("Setting project to geoanchor. Position: " + geoAnchor.position);
                Vector3 pos = geoAnchor.position;
                pos.y = uxManager.AppManager.ARSessionManager.GetDefaultPlaneY() ?? -1;
                projectOrigin.transform.position = pos;

                var euler = geoAnchor.rotation.eulerAngles;
                float y = euler.y + location.rotation;
                projectOrigin.transform.rotation = Quaternion.Euler(0, y, 0);
            }

        }
        #endregion Alignment

        /// <summary>   
        /// Creates 1x1 textures for all materials that have no texture.
        /// This is a solution for glb export since color-only materials are not exported.
        /// </summary>
        public void CreateGLBSafeTextures()
        {
            foreach (var resource in resources)
            {
                if (resource.gameObject != null)
                {
                    Material[] materials = resource.gameObject.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.sharedMaterials).ToArray();
                    foreach (Material material in materials)
                    {
                        if (material.mainTexture == null)
                            material.ColorToTexture();
                    }
                }
            }
        }
    }
}