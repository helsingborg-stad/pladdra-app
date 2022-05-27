using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARHandlers
{
    public class ARTrackImageHandler : IARHandler
    {
        private Action<ARTrackedImagesChangedEventArgs> OnHitHandler;
        public ARTrackImageHandler(Action<ARTrackedImagesChangedEventArgs> action)
        {
            OnHitHandler = action;
        }
        
        public ARTrackImageHandler(Action<ARTrackedImagesChangedEventArgs> action, Texture2D imageToTrack, float imageWidth)
        {
            var manager = UnityEngine.Object.FindObjectOfType<ARTrackedImageManager>();
            manager.referenceLibrary = manager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
            
            if (manager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(imageToTrack, "marker", imageWidth);
            }

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