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
        public struct LogEntry
        {
            public string timestamp;
            public string logString;
            public LogType type;
        }

        #region Public
        [SerializeField] int maxLogLength = 100;
        [SerializeField] int maxLogDisplayLength = 400;
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
        bool storeLog;
        bool sentReport;
        List<LogEntry> logEntries = new List<LogEntry>();
        #endregion Private

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
            if (PlayerPrefs.HasKey("log"))
                PlayerPrefs.DeleteKey("log");
        }

        void OnApplicationQuit()
        {
            if (PlayerPrefs.HasKey("log"))
                PlayerPrefs.DeleteKey("log");
        }

        void Start()
        {
            uiDocument.enabled = false;

        }

        void Update()
        {
            if (!sentReport)
            {// Send crash report on startup
                if (PlayerPrefs.HasKey("log"))
                {
                    var reports = CrashReport.reports;
                    string crashReport = "";
                    foreach (var report in reports)
                    {
                        crashReport += report.time + "---" + report.text + "<br>\n";
                    }

                    onSendLog.Invoke("Crash Report", DeviceInfo() + "<br>\nCrash report:<br>\n" + crashReport + "<br>\nLog:<br>\n" + FormatLog());

                    foreach (var report in reports)
                    {
                        report.Remove();
                    }

                    PlayerPrefs.DeleteKey("log");
                    sentReport = true;
                }
                PlayerPrefs.SetString("log", "");
                storeLog = true;
            }
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
                string myLog = "";
                foreach (LogEntry entry in logEntries)
                {
                    myLog += entry.timestamp + " " + entry.logString + "\n";
                }
                if (myLog.Length > maxLogDisplayLength)
                {
                    myLog = myLog.Substring(0, maxLogDisplayLength);
                }
                uiDocument.rootVisualElement.Q<Label>("log").text = myLog;


                uiDocument.rootVisualElement.Q<Button>("send").clicked += () =>
                {
                    onSendLog.Invoke("Debug log", DeviceInfo() + "<br>\n Log:<br>\n" + FormatLog());
                };
                uiDocument.rootVisualElement.Q<Button>("close").clicked += () =>
                            {
                                uiDocument.enabled = false;
                            };
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            if (logEntries.Count > maxLogLength)
            {
                logEntries.RemoveAt(logEntries.Count - 1);
            }
            logEntries.Insert(0, new LogEntry { timestamp = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "]", logString = logString, type = type });

            if (storeLog)
                PlayerPrefs.SetString("log", FormatLog());
        }

        string DeviceInfo()
        {
            return "Device type: " + SystemInfo.deviceType + "<br>\nDevice model: " + SystemInfo.deviceModel + "<br>\nOS: " + SystemInfo.operatingSystem + "<br>\n";
        }

        string FormatLog()
        {
            string myLogHTML = "";
            foreach (LogEntry entry in logEntries)
            {
                myLogHTML += (entry.type == LogType.Error ? "<font color='red'>" : "") + (entry.type == LogType.Warning ? "<font color='orange'>" : "") +
                entry.timestamp + " " + entry.logString + (entry.type == LogType.Error ? "</font>" : "") + (entry.type == LogType.Warning ? "</font>" : "") + "<br>\n";
            }
            return myLogHTML;
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
    }
}