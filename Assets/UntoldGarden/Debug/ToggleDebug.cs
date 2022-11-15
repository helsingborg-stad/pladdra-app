using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Toggles all debug objects either added in inspector or through adding AddToDebug helper script to the object
/// </summary>

namespace UntoldGarden.Util
{
    public class ToggleDebug : MonoBehaviour
    {
        public GameObject debugButton;
        public GameObject[] debugs;
        bool showDebug = false;

        public UnityEvent<bool> OnToggleDebug;

        // Start is called before the first frame update
        void Start()
        {
            Toggle(false);
        }

        public void DisableDebug()
        {
            debugButton.SetActive(false);
            foreach (GameObject go in debugs)
                go.SetActive(false);
        }

        public void Toggle(bool visualize = true)
        {
            if (showDebug)
            {
                foreach (GameObject go in debugs)
                    go.SetActive(true);
                OnToggleDebug.Invoke(true);
                showDebug = false;
            }
            else if (!showDebug)
            {
                foreach (GameObject go in debugs)
                    go.SetActive(false);
                OnToggleDebug.Invoke(false);
                showDebug = true;
            }
        }
    }
}