using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UntoldGarden
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
        [SerializeField] protected bool logCrashes = true;
        [SerializeField] protected bool sendLogOnError = false;
        [SerializeField] protected int maxLogLength = 100;
        [SerializeField] protected int maxLogDisplayLength = 400;
        public UnityEvent<string, string> onSendLog = new UnityEvent<string, string>();
        public UnityEvent<string, string> onSendHtmlLog = new UnityEvent<string, string>();

        #endregion Public

        #region Scene References
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        #endregion Scene References

        #region Private
        protected bool menuItemAdded = false;
        protected static string myLog = "";
        protected private string output;
        protected private string stack;
        protected bool storeLog = false;
        protected List<LogEntry> logEntries = new List<LogEntry>();
        #endregion Private

        protected void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        protected void OnDisable()
        {
            Application.logMessageReceived -= Log;
            if (PlayerPrefs.HasKey("log"))
                PlayerPrefs.DeleteKey("log");
        }

        protected void OnApplicationQuit()
        {
            if (PlayerPrefs.HasKey("log"))
                PlayerPrefs.DeleteKey("log");
        }

        protected void Start()
        {
            uiDocument.enabled = false;
        }

        protected virtual void Update()
        {
            if (!storeLog && logCrashes)
            {
                // Check for saved log and send it on startup
                if (PlayerPrefs.HasKey("log"))
                {
                    var reports = CrashReport.reports;
                    string crashReport = "";
                    foreach (var report in reports)
                    {
                        crashReport += report.time + "---" + report.text + "\n";
                    }

                    onSendLog.Invoke("Crash Report", DeviceInfo() + "\nCrash report:\n" + crashReport + "\nLog:\n" + PlayerPrefs.GetString("log"));
                    onSendHtmlLog.Invoke("Crash Report", DeviceInfoHTML() + "<br><br>Crash report:" + crashReport + "<br><br>Log:" + PlayerPrefs.GetString("log").Replace("\n", "<br>"));

                    foreach (var report in reports)
                    {
                        report.Remove();
                    }

                    PlayerPrefs.DeleteKey("log");
                }
                PlayerPrefs.SetString("log", "");
                storeLog = true;
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
                    onSendLog.Invoke("Debug log", DeviceInfo() + "\n Log:\n" + FormatLog());
                    onSendHtmlLog.Invoke("Debug log", DeviceInfoHTML() + "<br><br>Log:" + FormatHTMLLog());

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

            if(sendLogOnError && type == LogType.Error)
            {
                onSendLog.Invoke("Error", DeviceInfo() + "\n Log:\n" + FormatLog());
                onSendHtmlLog.Invoke("Error", DeviceInfoHTML() + "<br><br>Log:" + FormatHTMLLog());
            }

            if (storeLog)
                PlayerPrefs.SetString("log", FormatLog());
        }

        protected string DeviceInfo()
        {
            return "Device type: " + SystemInfo.deviceType + "\nDevice model: " + SystemInfo.deviceModel + "\nOS: " + SystemInfo.operatingSystem + "\n";
        }

        protected string DeviceInfoHTML()
        {
            return "Device type: " + SystemInfo.deviceType + "<br>Device model: " + SystemInfo.deviceModel + "<br>OS: " + SystemInfo.operatingSystem + "<br>";
        }

        protected string FormatLog()
        {
            string myLog = "";
            foreach (LogEntry entry in logEntries)
            {
                myLog += (entry.type == LogType.Error ? "<font color='red'>" : "") + (entry.type == LogType.Warning ? "<font color='orange'>" : "") +
                entry.timestamp + " " + entry.logString + (entry.type == LogType.Error ? "</font>" : "") + (entry.type == LogType.Warning ? "</font>" : "") + "\n";
            }
            return myLog;
        }

        protected string FormatHTMLLog()
        {
            string myLogHTML = "";
            foreach (LogEntry entry in logEntries)
            {
                myLogHTML += (entry.type == LogType.Error ? "<font color='red'>" : "") + (entry.type == LogType.Warning ? "<font color='orange'>" : "") +
                entry.timestamp + " " + entry.logString + (entry.type == LogType.Error ? "</font>" : "") + (entry.type == LogType.Warning ? "</font>" : "") + "<br>";
            }
            return myLogHTML;
        }
    }
}