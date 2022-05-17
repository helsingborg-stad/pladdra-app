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
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().trackedImagesChanged += OnImagesChanged;
        }

        private void OnImagesChanged(ARTrackedImagesChangedEventArgs args) => OnHitHandler(args);

        public void Deactivate()
        {
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().trackedImagesChanged -= OnImagesChanged;
            UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>().enabled = false;
        }
    }
}