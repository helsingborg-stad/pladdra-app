using System;
using System.Collections.Generic;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class LoadStandardProject : DialoguesUXHandler
    {
        public LoadStandardProject(DialoguesUXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("Loading Project " + uxManager.Project.name);
            // Disable the "Home" menu item during project load.
            uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", false);

            uxManager.GeospatialManager.GeospatialEnabled = false;
            uxManager.GeospatialManager.OnLocalizationUnsuccessful.RemoveAllListeners();

            DisplayProject();
        }

        #region Display Project

        void DisplayProject()
        {
            IUXHandler ux = new AllowUserToViewProject(uxManager);
            uxManager.UseUxHandler(ux);
        }


        // async void DisplayProject()
        // {
        //     Debug.Log("Display project");
        //     try
        //     {
        //         uxManager.UIManager.ShowLoading("Skapar projekt...");
        //         // Waiting for the project to load 
        //         await uxManager.Project.CreateProject();
        //         DisplayProjectInfo();
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("Exception: " + e.Message);
        //         uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", true);
        //         uxManager.UIManager.DisplayUI("warning-with-options", root =>
        //         {
        //             root.Q<Label>("warning").text = "Det gick inte att skapa projektet!";
        //             root.Q<Button>("option-one").clicked += () =>
        //             {
        //                 uxManager.AppManager.DisplayRecentProjectList(() => { uxManager.AppManager.LoadProjectCollections(); });
        //             };
        //             root.Q<Button>("option-one").text = "Återgå till projektmenyn";
        //             root.Q<Button>("option-two").style.display = DisplayStyle.None;
        //         });
        //     }
        // }
        // void DisplayProjectInfo()
        // {
        //     uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", true);

        //     uxManager.UIManager.DisplayUI("project-info", root =>
        //             {
        //                 root.Q<Label>("Name").text = uxManager.Project.name;
        //                 root.Q<Label>("Description").text = uxManager.Project.description;

        //                 Button start = root.Q<Button>("start");
        //                 if (uxManager.Project.hasLibraryResources || uxManager.Project.hasInteractiveResources)
        //                 {
        //                     start.clicked += () =>
        //                     {
        //                         uxManager.ShowWorkspaceDefault();
        //                     };
        //                 }
        //                 else
        //                 {
        //                     start.style.display = DisplayStyle.None;
        //                     Debug.Log($"Project has library resources {uxManager.Project.hasLibraryResources} and interactive resources {uxManager.Project.hasInteractiveResources}.");
        //                 }

        //                 Button proposals = root.Q<Button>("proposals");
        //                 if (uxManager.Project.hasProposals)
        //                 {
        //                     proposals.clicked += () =>
        //                     {
        //                         IUXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
        //                         uxManager.UseUxHandler(ux);
        //                     };
        //                 }
        //                 else
        //                 {
        //                     proposals.style.display = DisplayStyle.None;
        //                     Debug.Log($"Project has proposals {uxManager.Project.hasProposals}.");
        //                 }

        //             }
        //     );
        // }

        #endregion Display Project

        public override void Deactivate()
        {

        }
    }
}
