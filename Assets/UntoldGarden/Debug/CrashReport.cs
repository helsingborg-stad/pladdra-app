using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crashes : MonoBehaviour
{
    public UnityEvent<string,string> OnCrashReport = new UnityEvent<string,string>();

    void Start()
    {
        var reports = CrashReport.reports;
        string html = "";
        foreach (var report in reports)
        {
                html += report.time + "---" + report.text + "<br>";
        }

        OnCrashReport.Invoke("Crash report", "Device type: " +SystemInfo.deviceType + "<br>Device model: " + SystemInfo.deviceModel + "<br>OS: " + SystemInfo.operatingSystem + "<br><br>Log:" + html);

        foreach (var report in reports)
        {
            report.Remove();
        }

        
    }

}
