using System;
using UnityEngine.XR.ARFoundation;

namespace ARHandlers
{
    public class ARTrackImageHandler : IARHandler
    {
        private Action<ARTrackedImagesChangedEventArgs> OnHitHandler;
        public ARTrackImageHandler(Action<ARTrackedImagesChangedEventArgs> action)
        {
            OnHitHandler = action;
        }

        public void Activate()
        {
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().trackedImagesChanged += OnHitHandler;
        }

        public void Deactivate()
        {
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().trackedImagesChanged -= OnHitHandler;
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().enabled = false;
        }
    }
}