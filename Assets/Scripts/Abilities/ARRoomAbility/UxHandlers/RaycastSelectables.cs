using System;
using DefaultNamespace;
using Lean.Common;
using Piglet;
using UnityEngine;

namespace Abilities.ARRoomAbility.UxHandlers
{
    class RaycastSelectables : IRaycastHandler
    {
        private Action<bool, GameObject> OnHitCallback;
        public RaycastSelectables(Action<bool, GameObject> onHitCallback)
        {
            OnHitCallback = onHitCallback;
        }
        
        public void Activate()
        {
            Camera.main.gameObject.GetComponent<CameraRaycaster>().enabled = true;
            Camera.main.gameObject.GetComponent<CameraRaycaster>().OnRaycast += HandleRaycast;
        }
        
        private void HandleRaycast(bool success, RaycastHit hit) => OnHitCallback(success && IsValidRaycast(hit), success && IsValidRaycast(hit) ? hit.transform.gameObject : null);
        
        // Todo: Lift up Validation responsibility to callback && rename this class to something more general
        private bool IsValidRaycast(RaycastHit hit) => hit.transform.gameObject.TryGetComponent<LeanSelectable>(out LeanSelectable c) && c.enabled;
        
        public void Deactivate()
        {
            Camera.main.gameObject.GetComponent<CameraRaycaster>().OnRaycast -= HandleRaycast;
            Camera.main.gameObject.GetComponent<CameraRaycaster>().enabled = false;
        }
    }
}