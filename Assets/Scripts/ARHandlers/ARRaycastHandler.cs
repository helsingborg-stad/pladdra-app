using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARHandlers
{
    public class ARRaycastHandler : IARHandler
    {

        private ARRaycast raycast;
        private Action<ARRaycastUpdatedEventArgs> Callback;
        
        public ARRaycastHandler(Action<ARRaycastUpdatedEventArgs> callback)
        {
            Callback = callback;
        }

        public void Activate()
        {
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = true;
            raycast = UnityEngine.Object.FindObjectOfType<ARRaycastManager>()
                .AddRaycast(new Vector2(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2), 2.0F);

            raycast.updated += Callback;
        }

        public void Deactivate()
        {
            raycast.updated -= Callback;
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>()
                .RemoveRaycast(raycast);
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = false;
        }
    }
}