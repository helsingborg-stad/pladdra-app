using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.UX;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;

namespace Pladdra.ARSandbox.Dialogues
{
    public class DialoguesMenuManager : Pladdra.UI.MenuManager
    {
        ViewingModeManager viewingModeManager { get { return transform.parent.gameObject.GetComponentInChildren<ViewingModeManager>(); } }
        DialoguesUXManager dialoguesUXManager { get { return (DialoguesUXManager)uxManager; } }

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
                    IUXHandler ux = new AllowUserToViewZenMode(dialoguesUXManager);
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
                    dialoguesUXManager.Project.CreateGLBSafeTextures();
                    ExportGLB.AdvancedExport(dialoguesUXManager.Project.projectOrigin.gameObject, Application.persistentDataPath + "/" + dialoguesUXManager.Project.ProposalHandler.Proposal.name + ".glb");
                }
            });
        }
    }
}