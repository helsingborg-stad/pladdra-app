using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToChooseActionAfterSaving : UXHandler
    {
        public AllowUserToChooseActionAfterSaving(InteractionManager interactionManager)
        {
            this.interactionManager = interactionManager;
        }
        public override void Activate()
        {
            interactionManager.UIManager.ShowUI("save-confirmation", root =>
            {
                root.Q<Button>("continue").clicked += () =>
                {
                    interactionManager.ShowWorkspaceDefault();
                };
                root.Q<Button>("proposal-library").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewProposalLibrary(interactionManager);
                    interactionManager.UseUxHandler(ux);
                };
            });
        }
        public override void Deactivate()
        {

        }
    }
}
