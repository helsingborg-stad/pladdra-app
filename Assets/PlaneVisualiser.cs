using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace Pladdra.ARDebug
{
    /// <summary>
    /// Enables and disables the mesh renderer on all planes in the scene.
    /// Adds a menu item to toggle the planes.
    /// </summary>
    public class PlaneVisualiser : MonoBehaviour
    {
        public Material debugMat;
        public ARPlaneManager arPlaneManager;
        public MenuManager menuManager;
        List<GameObject> arObjects = new List<GameObject>();

        bool shown;
        // Start is called before the first frame update
        void Start()
        {
            if (arPlaneManager)
                arPlaneManager.planesChanged += UpdatePlaneList;

            if (menuManager)
            {
                menuManager.AddMenuItem(new MenuItem()
                {
                    id = "toggle-planes",
                    name = "AR Debug Viz",
                    action = () =>
                    {
                        TogglePlanes();
                    }
                });
            }
        }

        public void AddARObject(GameObject go)
        {
            arObjects.Add(go);
        }

        void UpdatePlaneList(ARPlanesChangedEventArgs args)
        {
            //add all new planes to list
            foreach (ARPlane plane in args.added)
            {
                arObjects.Add(plane.gameObject);
            }
            //remove all removed planes from list
            foreach (ARPlane plane in args.removed)
            {
                arObjects.Remove(plane.gameObject);
            }
        }

        /// <summary>
        /// Toggles ar debug material on all objects
        /// </summary>
        void TogglePlanes()
        {
            if (!shown)
            {
                foreach (GameObject go in arObjects)
                {
                    List<Material> mats = go.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                    mats.Add(debugMat);
                    go.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
                }
                shown = true;
            }
            else
            {
                foreach (GameObject go in arObjects)
                {
                    List<Material> mats = go.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                    mats.Remove(debugMat);
                    go.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
                }
                shown = false;
            }
        }
    }
}