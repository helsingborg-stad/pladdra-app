using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// ADDING A TEXT
/// </summary>

namespace UntoldGarden.AR
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ARSessionOrigin))]
    public class ARSessionManager : MonoBehaviour
    {
        # region Publix
        public ARSession session;
        [Header("Checks")]
        [SerializeField] bool checkTrackingState;
        [SerializeField] float pauseCheckAfterWarning = 10f;
        [Header("Default plane")]
        [SerializeField] GameObject defaultPlane;
        [SerializeField] bool updateDefaultPlane;
        [SerializeField] bool disableMeshRendererInBuild;

        #endregion Public

        #region Private
        ARAnchorManager anchorManager;
        ARPlaneManager planeManager;
        Transform user;
        Vector3 pastPosition;
        List<ARAnchor> anchors = new List<ARAnchor>();
        bool hasAnchor = false;
        [HideInInspector] public UnityEvent<Transform> OnSessionInitalisedAndTracking = new UnityEvent<Transform>();
        [HideInInspector] public UnityEvent<Transform> OnARPlane = new UnityEvent<Transform>();
        [HideInInspector] public UnityEvent OnLostTracking;
        [HideInInspector] public UnityEvent OnLowLightLevel;
        float maxCameraJump = .2f;
        float trackingWait = .2f;

        bool isTracking;
        #endregion Private

        void Start()
        {
            if (!session)
            {
                session = FindObjectOfType<ARSession>();
                if (!session)
                {
                    Debug.LogError("No ARSession!");
                    return;
                }
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            user = transform.GetChild(0);
            anchorManager = GetComponent<ARAnchorManager>();

            if (anchorManager)
            {
                anchorManager.anchorsChanged += AddAnchor;
            }

            if (defaultPlane)
            {
                Instantiate(defaultPlane, new Vector3(0, -1, 0), Quaternion.identity);
                defaultPlane.GetComponent<DefaultPlaneController>().Initialize(gameObject.GetComponent<ARPlaneManager>() ?? gameObject.AddComponent<ARPlaneManager>(), disableMeshRendererInBuild, updateDefaultPlane);
            }
            planeManager = GetComponent<ARPlaneManager>();
        }

        public void StartSession()
        {
            Start();
        }

        void OnDisable()
        {
            if (anchorManager) anchorManager.anchorsChanged -= AddAnchor;
        }

        private void Update()
        {
            if (checkTrackingState && isTracking) CheckTrackingState();
            if (user != null) pastPosition = user.position;
        }

        public void WaitForARSessionInit()
        {
            StartCoroutine(CrWaitForARSessionInit());
        }

        IEnumerator CrWaitForARSessionInit()
        {
            //UntoldGarden.AR.Logger.Log("SessionManager waiting for tracking");
#if UNITY_EDITOR
            yield return new WaitForSeconds(.5f);
#else
            while (session.subsystem.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                yield return null;

            string trackingState = session.subsystem.trackingState.ToString();
            string sessionID = session.subsystem.sessionId.ToString();
            UntoldGarden.AR.Logger.Log("SessionManager is tracking!");
#endif
            OnSessionInitalisedAndTracking.Invoke(user);
            isTracking = true;
        }

        //TODO Check light estimation

        #region Check TrackingState

        public void InitiateCheckTrackingState(float _maxCameraJump, float _trackingWait)
        {
            checkTrackingState = true;
            maxCameraJump = _maxCameraJump;
            trackingWait = _trackingWait;
        }

        private void CheckTrackingState()
        {
#if UNITY_EDITOR

            if (Vector3.Distance(user.position, pastPosition) > maxCameraJump)
                StartCoroutine(TrackingWarning());

#else
            if (session.subsystem.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                if (Vector3.Distance(user.position, pastPosition) > maxCameraJump)
                    StartCoroutine(TrackingWarning());
            }
            else
            {
                StartCoroutine(WaitForTracking());
            }
#endif
        }

        IEnumerator WaitForTracking()
        {
            yield return new WaitForSeconds(trackingWait);

            if (session.subsystem.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                StartCoroutine(TrackingWarning());
        }

        IEnumerator TrackingWarning()
        {
            Debug.Log("Tracking warning!" + pastPosition + ", " + user.position);
            checkTrackingState = false;
            OnLostTracking.Invoke();
            yield return new WaitForSeconds(pauseCheckAfterWarning);
            checkTrackingState = true;
        }
        #endregion

        #region Anchor

        // TODO Fix! Listens to ARAnchorManager.OnUpdateAnchor
        public void AddAnchor(ARAnchorsChangedEventArgs eventArgs)
        {
            if (eventArgs.added != null && eventArgs.added.Count > 0)
            {
                if (!hasAnchor) hasAnchor = true;

                foreach (ARAnchor anchor in eventArgs.added)
                {
                    anchors.Add(anchor);
                    if (anchor.sessionId != session.subsystem.sessionId)
                    {
                        UntoldGarden.AR.Logger.Log("Anchor was remoter! Anchor sessionID: " + anchor.sessionId);
                    }
                    else
                    {
                        UntoldGarden.AR.Logger.Log("Anchor was local. Anchor sessionID: " + anchor.sessionId);
                    }
                    anchor.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                UntoldGarden.AR.Logger.Log($"We have {anchors.Count} anchors");
            }
        }
        #endregion

        #region Public functions

        public bool IsTracking()
        {
            return isTracking;
        }

        public bool HasARPlane()
        {
            bool hasPlane = true;
#if UNITY_EDITOR
            // Debug.Log("In Unity Editor: HasARPlane: " + hasPlane);
#else
hasPlane  = planeManager != null && planeManager.trackables != null && planeManager.trackables.count > 0;
            // Debug.Log("In AR: HasARPlane: " + hasPlane);
#endif
            return hasPlane;
        }

        public Transform GetFirstAnchor()
        {
            Transform anchor = (anchors.Count > 0) ? anchors[0].transform : null;
            if (anchor == null) Debug.LogError("Can't get first anchor! There are no anchors!");
            return anchor;
        }

        // TODO Update to XROrigin
        public Transform GetUser()
        {
            if (!user)
                user = transform.GetChild(0);
            if (user)
                UntoldGarden.AR.Logger.Log("SessionManager get user " + user.name);
            else
                UntoldGarden.AR.Logger.Log("No user!");
            return user;
        }

        public bool HasAnchor()
        {
            return anchors.Count > 0;
        }

        public void ResetARSession()
        {
            UntoldGarden.AR.Logger.Log("Reset AR session");
            session.Reset();
        }

        #endregion
    }

}