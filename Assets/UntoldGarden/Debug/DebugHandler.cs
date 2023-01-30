using System.Collections;
using System.Collections.Generic;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra
{
    [RequireComponent(typeof(UIDocument))]
    public class DebugHandler : MonoBehaviour
    {
        #region Public
        [SerializeField] MenuManager menuManager;
        [SerializeField] UIManager uiManager;
        public UnityEvent<string, string> onSendLog = new UnityEvent<string, string>();
        #endregion Public

        #region Scene References
        UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        #endregion Scene References

        #region Private
        bool menuItemAdded = false;
        static string myLog = "";
        private string output;
        private string stack;
        #endregion Private

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        void Start()
        {
            uiDocument.enabled = false;
            uiManager.onError.AddListener(SendLog);
        }

        void Update()
        {
            if (!menuItemAdded)
            {
                AddMenuItem();
                menuItemAdded = true;
            }
        }

        public void ToggleDebugLog()
        {
            uiDocument.enabled = !uiDocument.enabled;
            if (uiDocument.enabled)
            {
                uiDocument.rootVisualElement.Q<Label>("log").text = myLog;
                uiDocument.rootVisualElement.Q<Button>("close").clicked += () =>
                            {
                                uiDocument.enabled = false;
                            };
                uiDocument.rootVisualElement.Q<Button>("send").clicked += () =>
                {
                    SendLog();
                };
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void AddMenuItem()
        {
            menuManager.AddMenuItem(new MenuItem()
            {
                name = "Debug",
                action = () =>
                {
                    ToggleDebugLog();
                }
            });
        }

        public void SendLog()
        {
            onSendLog.Invoke("Debug log", SystemInfo.operatingSystem + "\n" + "Build: " + " \n Log:" + myLog);
        }

    }
}