using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace DefaultNamespace
{
    public class CameraRaycaster : MonoBehaviour
    {
        private RaycastHit Hit;

        public Action<bool, RaycastHit> OnRaycast;

        public CameraRaycaster()
        {
            OnRaycast += (s, h) => { };
        }

        private void Update()
        {
            TryRaycast(OnRaycast);
        }
        
        void TryRaycast(Action<bool, RaycastHit> action) =>
            action(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)), out Hit), Hit);
    }
}