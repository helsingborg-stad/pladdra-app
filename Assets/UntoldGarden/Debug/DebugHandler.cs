using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UntoldGarden;

namespace Pladdra.DefaultAbility
{
    [RequireComponent(typeof(DisplayDebugLog))]
    public class DebugHandler : MonoBehaviour
    {
        [SerializeField] MenuManager menuManager;

        DisplayDebugLog debugLog { get { return GetComponent<DisplayDebugLog>(); } }
        bool menuItemAdded = false;

        void Update()
        {
            if (!menuItemAdded)
            {
                AddMenuItem();
                menuItemAdded = true;
            }
        }

        void AddMenuItem()
        {
            menuManager.AddMenuItem(new MenuItem()
            {
                name = "Debug",
                action = () =>
                {
                    debugLog.ToggleDebugLog();
                }
            });
        }


    }
}