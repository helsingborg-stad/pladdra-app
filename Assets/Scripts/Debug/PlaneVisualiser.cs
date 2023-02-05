using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Pladdra.ARDebug
{
    /// <summary>
    /// The `PlaneVisualiser` class manages the visualization of AR planes in the AR Foundation framework.
    /// </summary>
    public class PlaneVisualiser : MonoBehaviour
    {
        #region Public
        /// <summary>
        /// The Material that is used to visualize the AR planes.
        /// </summary>
        public Material debugMat;

        /// <summary>
        /// The `ARPlaneManager` component that manages the AR planes in the scene.
        /// </summary>
        public ARPlaneManager arPlaneManager;

        /// <summary>
        /// The `MenuManager` component that manages the user interface.
        /// </summary>
        public MenuManager menuManager;
        public bool randomizeColor = true;

        #endregion Public

        #region Private

        /// <summary>
        /// A list of all the AR objects in the scene, including the AR planes.
        /// </summary>
        List<GameObject> arObjects = new List<GameObject>();

        /// <summary>
        /// A flag that indicates whether the AR planes are currently shown.
        /// </summary>
        bool shown;

        Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };

        #endregion Private

        /// <summary>
        /// The Start method is called before the first frame update.
        /// </summary>
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

            // Set alpha to .5 for all colors in colors array
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = .5f;
            }
        }

        /// <summary>
        /// Adds a given GameObject to the list of AR objects.
        /// </summary>
        /// <param name="go">The GameObject to be added to the list of AR objects.</param>
        public void AddARObject(GameObject go)
        {
            arObjects.Add(go);
        }

        /// <summary>
        /// Updates the list of AR objects with new AR planes that have been added or removed.
        /// </summary>
        /// <param name="args">The ARPlanesChangedEventArgs that contain information about the added and removed AR planes.</param>
        void UpdatePlaneList(ARPlanesChangedEventArgs args)
        {
            // Add all new planes to the list
            foreach (ARPlane plane in args.added)
            {
                arObjects.Add(plane.gameObject);
            }
            // Remove all removed planes from the list
            foreach (ARPlane plane in args.removed)
            {
                arObjects.Remove(plane.gameObject);
            }
        }

        /// <summary>
        /// Toggles the visualization of the AR planes.
        /// </summary>
        void TogglePlanes()
        {
            if (!shown)
            {
                foreach (GameObject go in arObjects)
                {
                    List<Material> mats = go.GetComponent<MeshRenderer>().sharedMaterials.ToList();
                    if (randomizeColor) debugMat.color = colors[Random.Range(0, colors.Length)];
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