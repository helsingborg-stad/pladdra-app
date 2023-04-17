using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Pladdra.ARSandbox.Dialogues.Data;
using UntoldGarden.AR;

namespace Pladdra.ARSandbox.Dialogues
{
    [DisallowMultipleComponent]
    public class MarkerObjectController : MonoBehaviour
    {
        ARTrackedImage trackedImage;
        DialogueResource resource;
        ARSessionManager arSessionManager;
        Vector3 pos = Vector3.zero;
        bool onground = false;
        public void Init(DialogueResource resource, ARTrackedImage trackedImage, ARSessionManager arSessionManager = null)
        {
            Debug.Log("Init MarkerObjectController for " + resource.name);
            this.resource = resource;
            this.trackedImage = trackedImage;
            if (arSessionManager != null)
            {
                onground = true;
                this.arSessionManager = arSessionManager;
            }
        }

        private void Update()
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {

                pos = trackedImage.transform.position;
                if (onground)
                    pos.y = arSessionManager.GetDefaultPlaneY() ?? pos.y;
                transform.position = pos;
                transform.rotation = trackedImage.transform.rotation;
            }
        }
    }
}
