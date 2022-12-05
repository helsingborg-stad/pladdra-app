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
        public AllowUserToViewZenMode(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            interactionManager.UIManager.MenuManager.ToggleMenuButtonVisibility(false);
            interactionManager.UIManager.ShowUI("zen-mode", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    interactionManager.UIManager.ShowPreviousUI();
                    interactionManager.UIManager.MenuManager.ToggleMenuButtonVisibility(true);
                };
            });

        }
        public override void Deactivate()
        {

        }
    }
}
