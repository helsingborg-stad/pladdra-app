
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToViewZenMode: DialoguesUXHandler
    {
        public AllowUserToViewZenMode(DialoguesUXManager uxManager)
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
