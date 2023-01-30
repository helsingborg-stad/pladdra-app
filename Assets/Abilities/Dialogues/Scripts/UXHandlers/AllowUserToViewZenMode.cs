using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToViewZenMode : UXHandler
    {
        public AllowUserToViewZenMode(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("AllowUserToViewZenMode activated");
            uxManager.UIManager.MenuManager.ToggleMenuButtonVisibility(false);
            uxManager.UIManager.DisplayUI("zen-mode", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    Debug.Log("Close zen mode");
                    uxManager.UseLastUX();
                    uxManager.UIManager.MenuManager.ToggleMenuButtonVisibility(true);
                };
            });

        }
        public override void Deactivate()
        {
            Debug.Log("AllowUserToViewZenMode deactivated");
        }
    }
}
