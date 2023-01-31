// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using UntoldGarden.Util;

// namespace Marble
// {

//     [RequireComponent(typeof(ARTrackedImageManager))]
//     public class ReferenceImageLibraryManager : MonoBehaviour
//     {

//         //[SerializeField]
//         //private GameObject[] placeablePrefabs;

//         // private Dictionary<string, Post> spawnedPosts = new Dictionary<string, Post>(); // posts with markers
//         private Dictionary<string, Coroutine> removalCRs = new Dictionary<string, Coroutine>();
//         private ARTrackedImageManager trackedImageManager;


//         void Awake()
//         {
//             trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
//             trackedImageManager.trackedImagesChanged += ImageChanged;


//             //TODO: change so it spawns when needed
//             //foreach (GameObject prefab in placeablePrefabs)
//             //{
//             //    Post post = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<Post>();
//             //    post.name = prefab.name;
//             //    spawnedPosts.Add(prefab.name, post);
//             //    post.gameObject.SetActive(false);
//             //}
//             // Logging.Log(LogLevel.Mid, 1, "MutableImageLibrarySupport: " + DoesSupportMutableImageLibraries());

//         }
//         private void Start()
//         {
//             // list supported textures 
//             if (trackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
//                 for (int i = 0; i < mutableLibrary.supportedTextureFormatCount; i++)
//                 // Logging.Log( LogLevel.High, 0, "Texture Format Supported: " + mutableLibrary.GetSupportedTextureFormatAt(i));


//                 // foreach (Post post in spawnedPosts.Values)
//                 // AddImageToLibrary(post.marker, post.name, post.markerWidth);

//         }
//         public void SetImageLibrary(XRReferenceImageLibrary imgLibrary)
//         {
//             // Logging.Log(LogLevel.Mid, 2, "Setting image library to: " + imgLibrary.name);
//             trackedImageManager.enabled = false;
//             trackedImageManager.referenceLibrary = trackedImageManager.CreateRuntimeLibrary(imgLibrary);
//             trackedImageManager.enabled = true;
//         }

//         // public void AddPost(Post post)
//         // {
//         //     spawnedPosts.Add(post.name, post);
//         //     AddImageToLibrary(post.marker, post.name, post.markerWidth);
//         // }

//         public void AddImageToLibrary(Texture2D imageToAdd, string name = "new Image", float widthMeters = 0.5f)
//         {
//             if (trackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
//             {
//                 mutableLibrary.ScheduleAddImageWithValidationJob(
//                     imageToAdd,
//                     name,
//                     widthMeters);
//             }
//             // else
//                 // Logging.LogError(LogLevel.Mid, 1, "Reference image library isn't Mutable! (Cannot add images at runtime)");
//         }

//         bool DoesSupportMutableImageLibraries()
//         {
//             if (trackedImageManager.descriptor != null)
//                 return trackedImageManager.descriptor.supportsMutableLibrary;
//             else return false;
//         }

//         private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
//         {
//             foreach (ARTrackedImage trackedImage in eventArgs.added)
//             {
//                 // Logging.Log(LogLevel.High, 3, "Image added: " + trackedImage.name);

//                 if (removalCRs.ContainsKey(trackedImage.name))
//                 {
//                     StopCoroutine(removalCRs[trackedImage.name]);
//                     removalCRs.Remove(trackedImage.name);
//                 }

//                 UpdateImage(trackedImage);
//             }
//             foreach (ARTrackedImage trackedImage in eventArgs.updated)
//             {
//                 // Logging.Log(LogLevel.High, 4, "Image updated: " + trackedImage.name);
//                 UpdateImage(trackedImage);
//             }
//             foreach (ARTrackedImage trackedImage in eventArgs.removed)
//             {
//                 // Logging.Log(LogLevel.High, 5, "Image removed: " + trackedImage.name);
//                 if (!removalCRs.ContainsKey(trackedImage.name))
//                 {
//                     // Coroutine cr = StartCoroutine(ObjectRemovalTimer(trackedImage.name));
//                     removalCRs.Add(trackedImage.name, cr);
//                 }
//             }
//         }

//         // IEnumerator ObjectRemovalTimer(string objectName)
//         // {
//         //     Logging.Log(LogLevel.High, 5, "Starting removal timer for " + objectName);

//         //     yield return new WaitForSecondsRealtime(5);

//         //     //check if the image is being tracked. break if it is
//         //     foreach (ARTrackedImage image in trackedImageManager.trackables)
//         //         if (image.name == objectName && (image.trackingState == TrackingState.Tracking || image.trackingState == TrackingState.Limited))
//         //             yield break;

//         //     spawnedPosts[objectName].gameObject.SetActive(false);

//         // }
//         // private void UpdateImage(ARTrackedImage trackedImage)
//         // {
//         //     string name = trackedImage.referenceImage.name;

//         //     Post post = spawnedPosts[name];
//         //     Vector3 pos = trackedImage.transform.position;
//         //     switch (post.placement)
//         //     {
//         //         case Post.Placement.Floor:
//         //             //spawn on the floor under the marker
//         //             if (ARUtility.HasGroundPlane() == true)
//         //             {
//         //                 ARPlane plane = ARUtility.GetGroundPlane();
//         //                 post.transform.position = new Vector3(pos.x, plane.transform.position.y, pos.z);
//         //             }
//         //             else
//         //                 post.transform.position = pos;
//         //             break;
//         //         case Post.Placement.MarkerLock:
//         //             //change position and rotation
//         //             post.transform.position = pos;
//         //             post.transform.rotation = trackedImage.transform.rotation;
//         //             break;
//         //         case Post.Placement.MarkerZRotation:
//         //             post.transform.position = pos;
//         //             post.transform.rotation = Quaternion.Euler(post.transform.rotation.x, post.transform.rotation.y, trackedImage.transform.rotation.z);
//         //             break;
//         //         case Post.Placement.MarkerXRotation:
//         //             post.transform.position = pos;
//         //             post.transform.rotation = Quaternion.Euler(trackedImage.transform.rotation.x, post.transform.rotation.y, post.transform.rotation.z);
//         //             break;
//         //         case Post.Placement.MarkerYRotation:
//         //             post.transform.position = pos;
//         //             post.transform.rotation = Quaternion.Euler( post.transform.rotation.x, trackedImage.transform.rotation.y, post.transform.rotation.z);
//         //             break;
//         //         //case Post.Placement.Wall:
//         //         //    if (ARUtility.HasWall() == true)
//         //         //    {
//         //         //        ARPlane plane = ARUtility.GetClosestWall(trackedImage.transform.position);
//         //         //        post.transform.position = new Vector3(pos.x, plane.transform.position.y, pos.z);
//         //         //    }
//         //         //    else
//         //         //        post.transform.position = pos;
//         //         //    break;
//         //         default:
//         //             //only change position
//         //             post.transform.position = pos;
//         //             break;
//         //     }


//         //     post.gameObject.SetActive(true);

//         //     foreach (Post p in spawnedPosts.Values)
//         //         if (p.gameObject.name != name)
//         //             p.gameObject.SetActive(false);
//         // }
//     }

// }