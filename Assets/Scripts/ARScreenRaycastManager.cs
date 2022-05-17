using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace DefaultNamespace
{
    public class ARScreenRaycastManager : MonoBehaviour
    {
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        public UnityEvent<List<ARRaycastHit>> HitEvent;        
        
        public UnityEvent<List<ARRaycastHit>> MissEvent;
        
        private void Awake()
        {
            HitEvent.AddListener(hits => {});
            MissEvent.AddListener(hits => {});
        }

        private void Update()
        {
            var eventHandler = TryRaycast() ? HitEvent : MissEvent;
            eventHandler.Invoke(hits);
        }

        bool TryRaycast() =>
            GetComponent<ARRaycastManager>().Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits,
                TrackableType.PlaneWithinPolygon);
    }
}