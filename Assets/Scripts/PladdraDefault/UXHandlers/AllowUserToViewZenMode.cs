using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToViewZenMode : UXHandler
    {
        public AllowUserToViewZenMode(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.MenuManager.ToggleMenuButtonVisibility(false);
            uxManager.UIManager.ShowUI("zen-mode", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    uxManager.UIManager.ShowPreviousUI();
                    uxManager.UIManager.MenuManager.ToggleMenuButtonVisibility(true);
                };
            });

        }
        public override void Deactivate()
        {

        }
    }
}
