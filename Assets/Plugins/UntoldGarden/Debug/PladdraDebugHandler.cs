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
    public class PladdraDebugHandler : DebugHandler
    {

        #region Public
        [Header("Pladdra Implementation")]
        [SerializeField] MenuManager menuManager;
        #endregion Public
        
        protected override void Update()
        {
            base.Update();

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
                    ToggleDebugLog();
                }
            });
        }
    }
}