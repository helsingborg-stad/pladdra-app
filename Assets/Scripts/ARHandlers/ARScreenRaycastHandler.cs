using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace ARHandlers
{
    public class ARScreenRaycastHandler : IARHandler
    {

        private ARRaycast raycast;
        private UnityAction<List<ARRaycastHit>> OnHitAction;
        private UnityAction<List<ARRaycastHit>> OnMissAction;
        
        public ARScreenRaycastHandler(UnityAction<List<ARRaycastHit>> onHitAction)
        {
            OnHitAction = onHitAction;
            OnMissAction = hits => { };
        }

        public ARScreenRaycastHandler(UnityAction<List<ARRaycastHit>> onHitAction, UnityAction<List<ARRaycastHit>> onMissAction)
        {
            OnHitAction = onHitAction;
            OnMissAction = onMissAction;
        }

        public void Activate()
        {
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().HitEvent.AddListener(OnHitAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().FailedEvent.AddListener(OnMissAction);
        }

        public void Deactivate()
        {
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().HitEvent.RemoveListener(OnHitAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().FailedEvent.RemoveListener(OnMissAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().enabled = false;
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = false;
        }
    }
}