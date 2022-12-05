using System;
using UnityEngine.XR.ARFoundation;

namespace ARHandlers
{
    public class ARPlaneDetectionHandler : IARHandler
    {
        private Action<ARPlanesChangedEventArgs> Callback;

        public ARPlaneDetectionHandler(Action<ARPlanesChangedEventArgs> callback)
        {
            Callback = callback;
        }
        public ARPlaneDetectionHandler()
        {
            Callback = args => {};
        }

        public void Activate()
        {
            UnityEngine.Object.FindObjectOfType<ARPlaneManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARPlaneManager>().planesChanged += Callback;
        }

        public void Deactivate()
        {
            UnityEngine.Object.FindObjectOfType<ARPlaneManager>().planesChanged -= Callback;
            UnityEngine.Object.FindObjectOfType<ARPlaneManager>().enabled = false;
        }
    }
}