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
        private UnityAction<List<ARRaycastHit>> OnFailAction;
        
        public ARScreenRaycastHandler(UnityAction<List<ARRaycastHit>> hitAction)
        {
            OnHitAction = hitAction;
            OnFailAction = hits => { };
        }

        public ARScreenRaycastHandler(UnityAction<List<ARRaycastHit>> hitAction, UnityAction<List<ARRaycastHit>> failAction)
        {
            OnHitAction = hitAction;
            OnFailAction = failAction;
        }

        public void Activate()
        {
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().enabled = true;
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().HitEvent.AddListener(OnHitAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().FailedEvent.AddListener(OnFailAction);
        }

        public void Deactivate()
        {
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().HitEvent.RemoveListener(OnHitAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().FailedEvent.RemoveListener(OnFailAction);
            UnityEngine.Object.FindObjectOfType<ARScreenRaycastManager>().enabled = false;
            UnityEngine.Object.FindObjectOfType<ARRaycastManager>().enabled = false;
        }
    }
}