using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UntoldGarden.AR
{
    public class Logger : Singleton<Logger>
    {
        public bool enable;
        [SerializeField]
        Text m_LogText;
        [SerializeField]
        TMP_Text tmpText;

        public void SetTMPText(TMP_Text _tmp)
        {
            tmpText = _tmp;
            if (m_LogText.text != tmpText.text)
                tmpText.text = m_LogText.text;
        }

        public Text logText
        {
            get { return s_LogText; }
            set
            {
                m_LogText = value;
                s_LogText = value;
            }
        }

        [SerializeField]
        int m_VisibleMessageCount = 40;
        public int visibleMessageCount
        {
            get { return s_VisibleMessageCount; }
            set
            {
                m_VisibleMessageCount = value;
                s_VisibleMessageCount = value;
            }
        }

        int m_LastMessageCount;

        static int s_VisibleMessageCount;

        static Text s_LogText;

        static List<string> s_Log = new List<string>();

        static StringBuilder s_StringBuilder = new StringBuilder();

        //TODO Create log UI text here
        void Awake()
        {
            if (m_LogText != null)
                s_LogText = m_LogText;
            s_VisibleMessageCount = m_VisibleMessageCount;


            Log("Log console initialized.");
        }

        void Update()
        {
            if (!enable)
                return;

            lock (s_Log)
            {
                if (m_LastMessageCount != s_Log.Count)
                {
                    s_StringBuilder.Clear();
                    var startIndex = Mathf.Max(s_Log.Count - s_VisibleMessageCount, 0);
                    for (int i = startIndex; i < s_Log.Count; ++i)
                    {
                        s_StringBuilder.Append($"{i:000}> {s_Log[i]}\n");
                    }

                    string message = s_StringBuilder.ToString();

                    if (m_LogText != null)
                        s_LogText.text = message;
                    if (tmpText != null)
                        tmpText.SetText(message);

                }

                m_LastMessageCount = s_Log.Count;
            }
        }

        public static void Log(string message, bool sendToConsole = false)
        {

            message = Time.frameCount + ": " + message;
            lock (s_Log)
            {
                if (s_Log == null)
                    s_Log = new List<string>();

                s_Log.Add(message);
            }
            if (sendToConsole)
                Debug.Log("LOGGER: " + message);
        }
    }
}