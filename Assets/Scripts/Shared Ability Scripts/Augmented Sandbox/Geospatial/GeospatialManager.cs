using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Pladdra
{

    public class GeospatialManager : MonoBehaviour
    {
        public static GeospatialManager instance;

        public ARSession ARSession;
        public ARSessionOrigin SessionOrigin;
        public ARAnchorManager AnchorManager;
        public AREarthManager EarthManager;
        public ARCoreExtensions ARCoreExtensions;
        public Transform anchorContainer;
        public GameObject anchorPrefab;

        [Header("Events")]
        public UnityEvent OnLowLocalizationAccuracy;
        public UnityEvent OnLocalizationSuccessful;
        public UnityEvent OnLocalizationUnsuccessful;

        private Dictionary<string, GameObject> anchors = new Dictionary<string, GameObject>();


        [Header("Testing")]
        public GameObject debugPanel;
        public Text InfoText;
        public Text SnackBarText;
        public Text DebugText;


        bool geospatialSupported = false;


        private const string _localizingMessage = "Localizing your device to set anchor.";
        private const string _localizationInstructionMessage = "Point your camera at buildings, stores, and signs near you.";

        private const string _localizationFailureMessage =
            "Localization not possible.\n" +
            "Close and open the app to restart the session.";

        private const string _localizationSuccessMessage = "Localization completed.";


        private const float _timeoutSeconds = 180;
        private const double _orientationYawAccuracyThreshold = 25;

        private const double _horizontalAccuracyThreshold = 25;


        private bool _isInARView = false;
        public bool IsARActive { get { return _isInARView; } }
        private bool _isLocalizing = false;
        private bool _enablingGeospatial = false;
        private bool _lowAccuracyEventTriggered = false;

        private bool isLocalized = false;
        private bool _isReturning = false;

        private bool _waitingForLocationService;

        //private bool _shouldResolvingHistory = false;
        private float _localizationPassedTime = 0f;
        private float _configurePrepareTime = 3f;
        //private GeospatialAnchorHistoryCollection _historyCollection = null;
        //private List<GameObject> _anchorObjects = new List<GameObject>();


        private IEnumerator _startLocationService = null;
        private IEnumerator _asyncCheck = null;

        //bool isARCapable = false;
        bool isARCapable = true;

        // Start is called before the first frame update
        void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Application.targetFrameRate = 60;

            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;
        }


        public void OnEnable()
        {
            _startLocationService = StartLocationService();
            StartCoroutine(_startLocationService);

            _enablingGeospatial = false;

            _localizationPassedTime = 0f;
            _isLocalizing = true;
            _isReturning = false;

            if (SnackBarText != null)
                SnackBarText.text = _localizingMessage;

#if UNITY_IOS
            Debug.Log("Start location services.");
            Input.location.Start();
#endif
            if (debugPanel != null)
                debugPanel.SetActive(false);

            //LoadGeospatialAnchorHistory();
            //_shouldResolvingHistory = _historyCollection.Collection.Count > 0;
        }
        public void OnDisable()
        {
            if (_asyncCheck != null)
                StopCoroutine(_asyncCheck);
            _asyncCheck = null;
            if (_startLocationService != null)
                StopCoroutine(_startLocationService);
            _startLocationService = null;
            Debug.Log("Stop location services.");
            Input.location.Stop();

            // foreach (var anchor in _anchorObjects)
            // {
            //     Destroy(anchor);
            // }

            // _anchorObjects.Clear();
        }

        void Update()
        {
#if UNITY_IOS || UNITY_ANDROID

            UpdateDebugInfo();

            // Check session error status.
            LifecycleUpdate();
            if (_isReturning)
            {
                return;
            }

            if (ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                return;
            }

            // Check feature support and enable Geospatial API when it's supported.
            FeatureSupported featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            switch (featureSupport)
            {
                case FeatureSupported.Unknown:
                    return;
                case FeatureSupported.Unsupported:
                    ReturnWithReason("Geospatial API is not supported by this devices.");
                    return;
                case FeatureSupported.Supported:
                    if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                        GeospatialMode.Disabled)
                    {
                        Debug.Log("Geospatial sample switched to GeospatialMode. Enabled.");
                        ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode = GeospatialMode.Enabled;
                        _configurePrepareTime = 3.0f;
                        _enablingGeospatial = true;
                        geospatialSupported = true;
                        return;
                    }

                    break;
            }

            // Waiting for new configuration taking effect.
            if (_enablingGeospatial)
            {
                _configurePrepareTime -= Time.deltaTime;
                if (_configurePrepareTime < 0)
                {
                    _enablingGeospatial = false;
                }
                else
                {
                    return;
                }
            }

            // Check earth state.
            var earthState = EarthManager.EarthState;
            if (earthState == EarthState.ErrorEarthNotReady)
            {
                if (SnackBarText != null)
                    SnackBarText.text = "Initializing Geospatial functionalities.";
                return;
            }
            else if (earthState != EarthState.Enabled)
            {
                string errorMessage =
                    "Geospatial sample encountered an EarthState error: " + earthState;
                Debug.LogWarning(errorMessage);
                if (SnackBarText != null)
                    SnackBarText.text = errorMessage;
                return;
            }

            // Check earth localization.
#if UNITY_IOS
            bool isSessionReady = ARSession.state == ARSessionState.SessionTracking && Input.location.status == LocationServiceStatus.Running;
#else
            bool isSessionReady = ARSession.state == ARSessionState.SessionTracking;
#endif
            var earthTrackingState = EarthManager.EarthTrackingState;
            var pose = earthTrackingState == TrackingState.Tracking ?
                EarthManager.CameraGeospatialPose : new GeospatialPose();
            if (!isSessionReady || earthTrackingState != TrackingState.Tracking ||
                pose.OrientationYawAccuracy > _orientationYawAccuracyThreshold ||
                pose.HorizontalAccuracy > _horizontalAccuracyThreshold)
            {
                // Lost localization during the session.
                if (!_isLocalizing)
                {
                    _isLocalizing = true;
                    isLocalized = false;

                    _localizationPassedTime = 0f;

                    OnLowLocalizationAccuracy.Invoke();
                }

                if (_localizationPassedTime > _timeoutSeconds)
                {
                    Debug.LogError("Geospatial sample localization passed timeout.");
                    ReturnWithReason(_localizationFailureMessage);
                    OnLocalizationUnsuccessful.Invoke();

                }
                else
                {
                    _localizationPassedTime += Time.deltaTime;
                    if (SnackBarText != null)
                        SnackBarText.text = _localizationInstructionMessage;
                    if (!_lowAccuracyEventTriggered)
                        OnLowLocalizationAccuracy.Invoke();
                }
            }
            else if (_isLocalizing)
            {
                // Finished localization.
                _isLocalizing = false;
                _lowAccuracyEventTriggered = false;
                _localizationPassedTime = 0f;
                //SetAnchorButton.gameObject.SetActive(true);
                //ClearAllButton.gameObject.SetActive(_anchorObjects.Count > 0);
                if (SnackBarText != null)
                    SnackBarText.text = _localizationSuccessMessage;

                OnLocalizationSuccessful.Invoke();
                isLocalized = true;
                //ResolveHistory();

            }

            //InfoPanel.SetActive(true);
            if (earthTrackingState == TrackingState.Tracking && InfoText != null)
            {
                InfoText.text = string.Format(
                "Latitude/Longitude: {1}°, {2}°{0}" +
                "Horizontal Accuracy: {3}m{0}" +
                "Altitude: {4}m{0}" +
                "Vertical Accuracy: {5}m{0}" +
                "Eun Rotation: {6}{0}" +
                "Orientation Yaw Accuracy: {7}°",
                Environment.NewLine,
                pose.Latitude.ToString("F6"),
                pose.Longitude.ToString("F6"),
                pose.HorizontalAccuracy.ToString("F6"),
                pose.Altitude.ToString("F2"),
                pose.VerticalAccuracy.ToString("F2"),
                pose.EunRotation.ToString("F1"),
                pose.OrientationYawAccuracy.ToString("F1"));
            }
            else if (InfoText != null)
            {
                InfoText.text = "GEOSPATIAL POSE: not tracking";
            }
#endif


        }

        private void UpdateDebugInfo()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!Debug.isDebugBuild || EarthManager == null || DebugText == null)
            {
                return;
            }

            var pose = EarthManager.EarthState == EarthState.Enabled &&
                EarthManager.EarthTrackingState == TrackingState.Tracking ?
                EarthManager.CameraGeospatialPose : new GeospatialPose();
            var supported = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            DebugText.text =
                $"IsReturning: {_isReturning}\n" +
                $"IsLocalizing: {_isLocalizing}\n" +
                $"SessionState: {ARSession.state}\n" +
                $"LocationServiceStatus: {Input.location.status}\n" +
                $"FeatureSupported: {supported}\n" +
                $"EarthState: {EarthManager.EarthState}\n" +
                $"EarthTrackingState: {EarthManager.EarthTrackingState}\n" +
                $"  LAT/LNG: {pose.Latitude:F6}, {pose.Longitude:F6}\n" +
                $"  HorizontalAcc: {pose.HorizontalAccuracy:F6}\n" +
                $"  ALT: {pose.Altitude:F2}\n" +
                $"  VerticalAcc: {pose.VerticalAccuracy:F2}\n" +
                $". EunRotation: {pose.EunRotation:F2}\n" +
                $"  OrientationYawAcc: {pose.OrientationYawAccuracy:F2}";
#endif
        }
        private void LifecycleUpdate()
        {

#if UNITY_IOS || UNITY_ANDROID
            if (_isReturning)
            {
                return;
            }

            // Only allow the screen to sleep when not tracking.
            var sleepTimeout = SleepTimeout.NeverSleep;
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                sleepTimeout = SleepTimeout.SystemSetting;
            }

            Screen.sleepTimeout = sleepTimeout;

            // Quit the app if ARSession is in an error status.
            string returningReason = string.Empty;
            if (ARSession.state != ARSessionState.CheckingAvailability &&
                ARSession.state != ARSessionState.Ready &&
                ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                returningReason = string.Format(
                    "Geospatial sample encountered an ARSession error state {0}.\n" +
                    "Please start the app again.",
                    ARSession.state);
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                returningReason =
                    "Geospatial sample failed to start location service.\n" +
                    "Please start the app again and grant precise location permission.";
            }

            ReturnWithReason(returningReason);
#endif
        }

        public void ToggleDebugInfo()
        {
            if (debugPanel != null)
                debugPanel.SetActive(!debugPanel.activeSelf);
        }

        private IEnumerator AvailabilityCheck()
        {
            if (ARSession.state == ARSessionState.None)
            {
                Debug.Log("Checking ARCore or ARKit availability...");
                yield return ARSession.CheckAvailability();
            }

            // Waiting for ARSessionState.CheckingAvailability.
            yield return null;

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                Debug.Log("ARCore or ARKit is not installed. Installing...");
                yield return ARSession.Install();
            }

            // Waiting for ARSessionState.Installing.
            yield return null;

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Debug.Log("Requesting camera permission.");
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(3.0f);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                // User has denied the request.
                Debug.LogWarning(
                    "Failed to get camera permission. VPS availability check is not available.");
                yield break;
            }
#endif

            while (_waitingForLocationService)
            {
                yield return null;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarning(
                    "Location service is not running. VPS availability check is not available.");
                yield break;
            }

            // Update event is executed before coroutines so it checks the latest error states.
            if (_isReturning)
            {
                yield break;
            }

            var location = Input.location.lastData;
            Debug.LogFormat("Checking VPS availability at ({0}, {1})...", location.latitude, location.longitude);
            var vpsAvailabilityPromise =
                    AREarthManager.CheckVpsAvailability(location.latitude, location.longitude);
            yield return vpsAvailabilityPromise;

            Debug.LogFormat("VPS Availability at ({0}, {1}): {2}",
                location.latitude, location.longitude, vpsAvailabilityPromise.Result);
            // VPSCheckCanvas.SetActive(vpsAvailabilityPromise.Result != VpsAvailability.Available);


        }
        private IEnumerator StartLocationService()
        {
            _waitingForLocationService = true;
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Requesting fine location permission.");
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(3.0f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service is disabled by User.");
                _waitingForLocationService = false;
                yield break;
            }

            Debug.Log("Start location service.");
            Input.location.Start();

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return null;
            }

            _waitingForLocationService = false;
            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarningFormat(
                    "Location service ends with {0} status.", Input.location.status);
                Input.location.Stop();
            }
        }
        private void ReturnWithReason(string reason)
        {

            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            //SetAnchorButton.gameObject.SetActive(false);
            //ClearAllButton.gameObject.SetActive(false);
            //InfoPanel.SetActive(false);
#if !UNITY_EDITOR
            Debug.LogError(reason);
#endif
            if (SnackBarText != null)
                SnackBarText.text = reason;
            _isReturning = true;
            //Invoke(nameof(QuitApplication), _errorDisplaySeconds);
        }

        //Place anchor at camera geolocation
        public void PlaceGeoAnchorAtCameraPose(string id, bool useCameraRotation, Action<bool, string, GameObject> callback)
        {

            var pose = EarthManager.CameraGeospatialPose;

            var anchor = AnchorManager.AddAnchor(pose.Latitude, pose.Longitude, pose.Altitude, useCameraRotation ? pose.EunRotation : Quaternion.identity);
            if (anchor == null)
            {
                Debug.LogError("Failed to add anchor.");
                callback?.Invoke(false, "Failed to add anchor.", null);
                return;
            }

            if (anchorContainer != null)
                anchor.transform.parent = anchorContainer;

            if (anchorPrefab != null)
            {
                var anchorObject = Instantiate(anchorPrefab, anchor.transform);
                anchorObject.transform.localPosition = Vector3.zero;
                anchorObject.transform.localRotation = Quaternion.identity;
            }

            anchors.Add(id, anchor.gameObject);

            float acc = Mathf.CeilToInt((float)EarthManager.CameraGeospatialPose.HorizontalAccuracy * 100) / 100;
            callback?.Invoke(true, "Anchor sucessfuly added at lat " + pose.Latitude + " lng " + pose.Longitude + ". Horizontal localization accuracy: " + acc + "m", anchor.gameObject);
        }
        // Place anchor at geolocation
        public void PlaceGeoAnchorAtLocation(string id, double latitude, double longitude, double altitude, Quaternion rotation, Action<bool, string, GameObject> callback)
        {
            var anchor = AnchorManager.AddAnchor(latitude, longitude, altitude, rotation);
            if (anchor == null)
            {
                Debug.LogError("Failed to add anchor.");
                callback?.Invoke(false, "Failed to add anchor.", null);
                return;
            }

            if (anchorContainer != null)
                anchor.transform.parent = anchorContainer;

            if (anchorPrefab != null)
            {
                var anchorObject = Instantiate(anchorPrefab, anchor.transform);
                anchorObject.transform.localPosition = Vector3.zero;
                anchorObject.transform.localRotation = Quaternion.identity;
            }

            anchors.Add(id, anchor.gameObject);
            float acc = Mathf.CeilToInt((float)EarthManager.CameraGeospatialPose.HorizontalAccuracy * 100) / 100;
            callback?.Invoke(true, "Anchor sucessfuly added at lat " + latitude + " lng " + longitude + ". Horizontal localization accuracy: " + acc + "m", anchor.gameObject);
        }
        // Place anchor at lat long with camera altitude
        public async void PlaceGeoAnchorAtLocation(string id, double latitude, double longitude, Quaternion rotation, Action<bool, string, GameObject> callback)
        {
            Debug.Log("Placing anchor at location");
#if UNITY_EDITOR
            callback?.Invoke(false, "Not mobile player.", null);
            return;
#endif
            bool debugLocalisation = true;
            while (!isLocalized)
            {
                if (debugLocalisation) Debug.Log("Waiting for localization");
                debugLocalisation = false;
                await Task.Yield();
            }

            Debug.Log("Is localized");
            var anchor = AnchorManager.AddAnchor(latitude, longitude, EarthManager.CameraGeospatialPose.Altitude, rotation);
            if (anchor == null)
            {
                Debug.LogError("Failed to add anchor.");
                callback?.Invoke(false, "Failed to add anchor.", null);
                return;
            }

            if (anchorContainer != null)
                anchor.transform.parent = anchorContainer;

            if (anchorPrefab != null)
            {
                var anchorObject = Instantiate(anchorPrefab, anchor.transform);
                anchorObject.transform.localPosition = Vector3.zero;
                anchorObject.transform.localRotation = Quaternion.identity;
            }

            anchors.Add(id, anchor.gameObject);
            float acc = Mathf.CeilToInt((float)EarthManager.CameraGeospatialPose.HorizontalAccuracy * 100) / 100;
            callback?.Invoke(true, "Anchor successfuly added at lat " + latitude + " lng " + longitude + ". Horizontal localization accuracy: " + acc + "m", anchor.gameObject);
        }
        public GameObject GetGeoAnchor(string id)
        {
            if (anchors.ContainsKey(id))
            {
                return anchors[id];
            }
            Debug.LogError("Anchor not found");
            return null;
        }
    }
}