using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UntoldGarden.AR;
using UntoldGarden.Utils;

namespace Pladdra.DialogueAbility
{
    //TODO Clean
    //TODO Create this one if needed
    public class ARReferenceImageHandler : MonoBehaviour
    {
        public GameObject prefab;
        public ARSessionManager arSessionManager;
        GameObject currentlyTrackedImage;
        public GameObject CurrentlyTrackedImage { get { return currentlyTrackedImage; } }
        // Start is called before the first frame update
        public ARTrackedImageManager arTrackedImageManager;
        MenuManager menuManager { get { return transform.parent.gameObject.GetComponentInChildren<MenuManager>(); } }
        ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }
        Dictionary<ARTrackedImage, GameObject> trackedImages = new Dictionary<ARTrackedImage, GameObject>();
        bool addedMenuItem = false;
        void Start()
        {
            arTrackedImageManager.trackedImagesChanged += ImageChanged;

        }
        public void AddReferenceImage(Texture2D image,string id, float width)
        {
            // Debug.Log("AddImage " + image.name);
            StartCoroutine(AddImageCoroutine(image, id));
        }
        IEnumerator AddImageCoroutine(Texture2D image, string id)
        {
            // Debug.Log("AddImageCoroutine");
            image = image.ChangeFormat(TextureFormat.RGBA32);
            yield return null;

            if (arTrackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                Debug.Log("Try to add image " + image.name + " texture sixe is y " + image.width + "x" + image.height);
                var jobHandle = mutableLibrary.ScheduleAddImageWithValidationJob(image, id, 0.21f);
                yield return new WaitUntil(() => jobHandle.jobHandle.IsCompleted);
            }
            else
            {
                Debug.Log("Not a MutableRuntimeReferenceImageLibrary");
            }

            // Debug.Log("AddImageCoroutine done");
        }

        private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
                AddImage(trackedImage);
            }
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                // Logging.Log(LogLevel.High, 4, "Image updated: " + trackedImage.name);
                UpdateImage(trackedImage);
            }
            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
                RemoveImage(trackedImage);
                // Logging.Log(LogLevel.High, 5, "Image removed: " + trackedImage.name);
                // if (!removalCRs.ContainsKey(trackedImage.name))
                // {
                //     Coroutine cr = StartCoroutine(ObjectRemovalTimer(trackedImage.name));
                //     removalCRs.Add(trackedImage.name, cr);
                // }
            }
        }

        private void AddImage(ARTrackedImage trackedImage)
        {
            if (!trackedImages.ContainsKey(trackedImage))
            {
                Debug.Log("New tracked image! " + trackedImage.name);
                GameObject go = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
                UnityEngine.Object.FindObjectOfType<Pladdra.ARDebug.PlaneVisualiser>().AddARObject(go); //Temporary solution for AR viz

                // trackedImage.referenceImage.name

                trackedImages.Add(trackedImage, go);
                currentlyTrackedImage = go;

                //only for 
                if (!addedMenuItem)
                {
                    menuManager.AddMenuItem(new MenuItem()
                    {
                        id = "alignToARMarker",
                        name = "Placera på markör",
                        action = () =>
                        {
                            projectManager.Project.AlignToARMarker(trackedImage.transform.position, trackedImage.transform.rotation);
                        }
                    });
                    addedMenuItem = true;
                }
            }
            else
            {
                Debug.Log("Tracked image already exists! " + trackedImage.name);
            }
        }
        private void UpdateImage(ARTrackedImage trackedImage)
        {
            if (trackedImages.ContainsKey(trackedImage))
            {
                trackedImages[trackedImage].transform.position = trackedImage.transform.position;
                trackedImages[trackedImage].transform.rotation = trackedImage.transform.rotation;
            }
            else
            {
                Debug.Log("No tracked image found for " + trackedImage.name);
            }
        }

        private void RemoveImage(ARTrackedImage trackedImage)
        {
            if (trackedImages.ContainsKey(trackedImage))
            {
                Destroy(trackedImages[trackedImage]);
                trackedImages.Remove(trackedImage);
            }
        }
    }
}