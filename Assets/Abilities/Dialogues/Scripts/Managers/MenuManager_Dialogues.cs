using System.Collections;
using System.Collections.Generic;
using Pladdra.DialogueAbility.UX;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;

namespace Pladdra.DialogueAbility
{
    public class MenuManager_Dialogues : Pladdra.UI.MenuManager
    {
        ViewingModeManager viewingModeManager { get { return transform.parent.gameObject.GetComponentInChildren<ViewingModeManager>(); } }

        protected override void Start()
        {
            base.Start();
            menuItems.Add(new MenuItem()
            {
                id = "project-list",
                name = "Hem",
                action = () =>
                {
                    appManager.DisplayRecentProjectList(() => { appManager.LoadProjectCollections(); });
                    ToggleMenu(false);
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "zen-mode",
                name = "Zenmode", // TODO Change to call from UItexts
                action = () =>
                {
                    Debug.Log("Zenmode clicked");
                    ToggleMenu(false);
                    UXHandler ux = new AllowUserToViewZenMode(uxManager);
                    uxManager.UseUxHandler(ux);
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "white-mode",
                name = "Whitemode", // TODO Change to call from UItexts
                action = () =>
                {
                    viewingModeManager.ToggleWhiteSphere();
                    ToggleMenu(true);

                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "fade-mode",
                name = "Fademode", // TODO Change to call from UItexts
                action = () =>
                {
                    viewingModeManager.ToggleFadeMode();
                    ToggleMenu(true);
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "export-glb",
                name = "Exportera GLB",
                action = () =>
                {
                    //TODO Disable button while exporting
                    ExportGLB.AdvancedExport(uxManager.Project.projectOrigin.gameObject, Application.persistentDataPath + "/" + uxManager.Project.ProposalHandler.Proposal.name + ".glb");
                }
            });
        }
    }
}