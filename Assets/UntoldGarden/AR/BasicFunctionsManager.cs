using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UntoldGarden.AR;
using UntoldGarden.Util;

/// <summary>
/// Keeps track of all general stats and top level game logic
///
/// Changes language globally and stores this
///
/// Checks and warns for:
/// * Low battery
/// * Initiates tracking state check in ARSessionManager
/// * Initiates light level check in ARSessionManager (not implemented)
/// * Geofencing (stops users from leaving a specific area)
///
/// Sets sleep timeout to NeverSleep
///
/// Toggles global debug
///
/// Global audio settings
/// Checks for headphones
///
/// Reload scene
///
/// Delete playerprefs
/// 
/// </summary>
/// 

namespace UntoldGarden
{
    [DisallowMultipleComponent]
    public class BasicFunctionsManager : MonoBehaviour
    {
        [HideInInspector] public enum GameControl { Quit, Restart, Pause }
        [HideInInspector] public enum Warnings { None, Geofencing, Battery, Lighting, Tracking }

        [Header("Debug")]
        [Tooltip("Sets debug value for full project")]
        public bool globalDebug;
        public ToggleDebug toggleDebug;
        public AR.Logger logger;

        [Header("Audio")]
        public bool useWwise = false;
        [Tooltip("Can we play sounds through the speaker audio?")]
        public static bool headphonesOnly = false;

        [Header("Language")]
        [SerializeField] bool useLanguage;
        [SerializeField] bool forceMainLanguage;
        public static bool mainLanguage = true;

        [Header("Battery")]
        [SerializeField] bool checkBatteryLevel = true;
        [SerializeField] float minBatteryLevel = .2f;

        // [Header("Lighting")]
        // [SerializeField] bool checkLighting = true;
        // [SerializeField] float minLighting = .2f;

        [Header("Camera")]
        [SerializeField] bool checkTrackingState = true;
        [SerializeField] float maxCameraJump = .2f;
        [SerializeField] float trackingWait = .2f;

        Transform user;
        ARSessionManager arSessionManager;

        [Header("Geofencing")]
        [SerializeField] float maxDistanceFromOrigin;

        public UnityEvent<bool> OnChangeLanguage;
        public UnityEvent<Warnings> OnWarning;

        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            arSessionManager = FindObjectOfType<ARSessionManager>();

            if (!globalDebug)
            {
                if (toggleDebug)
                    toggleDebug.DisableDebug();
                if (logger)
                    logger.enable = false;
            }
        }

        private void Start()
        {
            if (useLanguage && !forceMainLanguage)
            {
                if (!PlayerPrefs.HasKey("language"))
                    mainLanguage = Application.systemLanguage == SystemLanguage.Swedish;
                else if (PlayerPrefs.HasKey("language"))
                    mainLanguage = (PlayerPrefs.GetInt("language") == 0);

                Debug.Log("Language is " + mainLanguage);
                OnChangeLanguage.Invoke(mainLanguage);
            }

            if (checkBatteryLevel)
                InvokeRepeating(nameof(CheckBatteryLevel), 60, 60);

            if (checkTrackingState && arSessionManager)
            {
                arSessionManager.InitiateCheckTrackingState(maxCameraJump, trackingWait);
                arSessionManager.OnLostTracking.AddListener(delegate
                {
                    OnWarning.Invoke(Warnings.Tracking);
                });
            }
        }

        private void CheckBatteryLevel()
        {
#if !UNITY_EDITOR
            if (SystemInfo.batteryLevel < minBatteryLevel)
            {
                OnWarning.Invoke(Warnings.Battery);
                checkBatteryLevel = false; // We don't need to warn again
            }
#endif
            //TODO add so we can check this multiple times in one session
        }

        public void ChangeLanguage()
        {
            Debug.Log("Changing language");
            if (mainLanguage)
                mainLanguage = false;
            else
                mainLanguage = true;

            if (mainLanguage)
                PlayerPrefs.SetInt("language", 0);
            else
                PlayerPrefs.SetInt("language", 1);

            Debug.Log("Invoke with " + mainLanguage);
            OnChangeLanguage.Invoke(mainLanguage);
        }

        private void CallGameControl(GameControl call)
        {
            switch (call)
            {
                case GameControl.Quit:
                    DoQuit();
                    break;
                case GameControl.Restart:
                    ReloadScene();
                    break;
                case GameControl.Pause:
                    throw new NotImplementedException();
            }
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void DoQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void DeletePlayerPrefs()
        {
            Debug.Log("Deleted all Player Prefs");
            PlayerPrefs.DeleteAll();
            ReloadScene();
        }
        
        public void OpenWebPage(string _url)
        {
            Application.OpenURL(_url);
        }

        Vector3 origin;
        public void ApplyLocalGeofencing(Vector3 _origin, float repeatRate)
        {
            origin = _origin;

            if (user == null)
                user = arSessionManager.GetUser();
            InvokeRepeating(nameof(CheckLocalGeofencing), 0, repeatRate);
        }

        public void CheckLocalGeofencing()
        {
            if (Vector3.Distance(user.position, origin) > maxDistanceFromOrigin)
            {
                OnWarning.Invoke(Warnings.Geofencing);
            }
        }
    }
}