using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UntoldGarden.AR
{
    public class DefaultPlaneController : MonoBehaviour
    {
        #region Public
        [Tooltip("Looks for ARSessionOrigin and adds a ARPlaneManager if none is found.")]
        [SerializeField] bool initOnStart = false;
        [Tooltip("Updates the default plane when a new plane is detected. Can be controlled via ToggleUpdate")]
        [SerializeField] bool update = true;
        [Tooltip("Adds this as the ARPlaneManagers plane prefab.")]
        [SerializeField] GameObject arPlanePrefab;
        [Header("Debug")]
        [Tooltip("In case the default plane has a material that shouldn't be shown in build. Don't use in case there is only the shadowPlaneMaterial.")]
        [SerializeField] bool disableMeshRenderInBuild = false;
        [SerializeField] bool debugLog;
        #endregion Public

        #region Private
        ARPlaneManager planeManager;
        float y = 0;
        Vector3 bottom;
        List<GameObject> planes = new List<GameObject>();

        #endregion Private

        void Start()
        {
            if (initOnStart)
            {
                ARPlaneManager arPlaneManager = FindObjectOfType<ARSessionOrigin>().GetComponent<ARPlaneManager>() ?? FindObjectOfType<ARSessionOrigin>().gameObject.AddComponent<ARPlaneManager>();
                planeManager.planePrefab = arPlanePrefab;
                Initialize(arPlaneManager, disableMeshRenderInBuild, update);
            }
        }

        public void Initialize(ARPlaneManager planeManager, bool disableMeshRendererInBuild, bool update)
        {
            this.planeManager = planeManager;
            this.update = update;
            this.disableMeshRenderInBuild = disableMeshRendererInBuild;

            if (planeManager)
                planeManager.planesChanged += MoveDefaultPlane;

#if !UNITY_EDITOR
            if (disableMeshRenderInBuild)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
#endif
        }

        // Lowers the default plane to the lowest point on the ARPlane meshes
        private void MoveDefaultPlane(ARPlanesChangedEventArgs args)
        {
            if (!update)
                return;

            foreach (ARPlane plane in args.added)
            {
                planes.Add(plane.gameObject);
            }
            foreach (ARPlane plane in args.updated)
            {
                planes[planes.IndexOf(plane.gameObject)] = plane.gameObject;
            }
            foreach (ARPlane plane in args.removed)
            {
                planes.Remove(plane.gameObject);
            }

            try
            {
                Vector3[] verts;
                Vector3 vecc;
                foreach (GameObject go in planes)
                {
                    verts = go.GetComponent<MeshFilter>().sharedMesh.vertices;
                    foreach (Vector3 vec in verts)
                    {
                        vecc = vec + go.transform.position;
                        if (vecc.y < bottom.y)
                            bottom = vecc;
                    }
                }
            }
            catch { }

            //Adding a small offset so it doesnt update on every tenth of a milimeter
            if (bottom.y < (y - .01f))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, bottom.y, gameObject.transform.position.z);
                if (debugLog) UntoldGarden.AR.Logger.Log($"{bottom.y} is less than {y}, move Default Plane to {gameObject.transform.position}");
                y = bottom.y; // To still move default plane up in case it would be instantiated below the lowest ARPlane point before any ARPlane was added
            }
        }

        public void ToggleUpdate(bool update)
        {
            this.update = update;
        }
    }
}