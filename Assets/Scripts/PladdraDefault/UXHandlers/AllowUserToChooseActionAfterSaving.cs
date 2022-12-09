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
        public AllowUserToChooseActionAfterSaving(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            uxManager.UIManager.ShowUI("save-confirmation", root =>
            {
                root.Q<Button>("continue").clicked += () =>
                {
                    uxManager.ShowWorkspaceDefault();
                };
                root.Q<Button>("proposal-library").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
            });
        }
        public override void Deactivate()
        {

        }
    }
}
