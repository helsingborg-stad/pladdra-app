using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Pladdra.ARSandbox.Dialogues.Data;

namespace Pladdra.ARSandbox.Dialogues
{
    [DisallowMultipleComponent]
    public class MarkerObjectController : MonoBehaviour
    {
        ARTrackedImage trackedImage;
        DialogueResource resource;
        public void Init(DialogueResource resource, ARTrackedImage trackedImage)
        {
            Debug.Log("Init MarkerObjectController for " + resource.name);
            this.resource = resource;
            this.trackedImage = trackedImage;
        }

        private void Update()
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                transform.position = trackedImage.transform.position;
                transform.rotation = trackedImage.transform.rotation;
            }
        }
    }
}
