using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UntoldGarden
{
    [RequireComponent(typeof(UIDocument))]
    public class DisplayDebugLog : MonoBehaviour
    {
        static string myLog = "";
        private string output;
        private string stack;
        UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        public UnityEvent<string,string> onSendLog = new UnityEvent<string,string>();

        void Start()
        {
            uiDocument.enabled = false;
        }
        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
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
                    onSendLog.Invoke("Debug log", SystemInfo.operatingSystem + "\n Log:" + myLog);
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
    }
}