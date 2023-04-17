using System;
using System.Collections.Generic;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class LoadMarkerProject : DialoguesUXHandler
    {
        public LoadMarkerProject(DialoguesUXManager uxManager)
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

            Debug.Log("Markerbased project: Project requires marker to be displayed.");
            uxManager.UIManager.DisplayUI("look-for-marker");
            WaitForMarker();
        }

        async void WaitForMarker()
        {
            uxManager.Project.CreateProjectContainers();
            await uxManager.Project.CreateProjectMarker();

            bool debug = true;
            while (!uxManager.Project.hasTrackerMarker)
            {
                if (debug)
                {
                    Debug.Log("Waiting for marker");
                    debug = false;
                }
                await System.Threading.Tasks.Task.Delay(100);
            }
            Debug.Log("Marker found");
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
        //                     start.style.visibility = Visibility.Hidden;
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
        //                     proposals.style.visibility = Visibility.Hidden;
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
