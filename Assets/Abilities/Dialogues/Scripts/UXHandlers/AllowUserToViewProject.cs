using System;
using System.Collections.Generic;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToViewProject : DialoguesUXHandler
    {
        public AllowUserToViewProject(DialoguesUXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("Init Project " + uxManager.Project.name);
            // Disable the "Home" menu item during project load.
            uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", false);
            DisplayProject();

            // //TODO Split into UXHandler AllowUserToViewStandardProject, AllowUserToViewGeolocatedProject and AllowUserToViewMarkerProject
            // if (uxManager.Project.requiresGeolocation)
            // {
            //     uxManager.UIManager.DisplayUI("prompt", root =>
            //     {
            //         root.Q<Label>("prompt").text = "Titta dig omkring för att lokalisera AR.";
            //         root.Q<Button>("option-one").clicked += () =>
            //         {
            //             uxManager.Project.overrideGeolocation = true;
            //             DisplayProject();
            //         };
            //         root.Q<Button>("option-one").text = "Skippa geolokalisering";
            //     });
            //     Debug.Log("Project requires location to be displayed.");
            //     Action<bool, string, GameObject> placedObject = SetGeoAnchorAndDisplayProject;
            //     uxManager.GeospatialManager.GeospatialEnabled = true;
            //     uxManager.GeospatialManager.OnLocalizationUnsuccessful.AddListener(GeolocationUnsuccessful);
            //     uxManager.GeospatialManager.PlaceGeoAnchorAtLocation("id", uxManager.Project.location.lat, uxManager.Project.location.lon, Quaternion.identity, placedObject);
            // }
            // else
            // {
            //     uxManager.GeospatialManager.GeospatialEnabled = false;
            //     uxManager.GeospatialManager.OnLocalizationUnsuccessful.RemoveAllListeners();
            //     if (uxManager.Project.marker.required)
            //     {
            //         Debug.Log("Project requires marker to be displayed.");
            //         uxManager.UIManager.DisplayUI("look-for-marker");
            //         WaitForMarker();
            //     }
            //     else
            //     {
            //         DisplayProject();
            //     }
            // }
        }

        // #region Marker

        // async void WaitForMarker()
        // {
        //     uxManager.Project.CreateProjectContainers();
        //     await uxManager.Project.CreateProjectMarker();

        //     bool debug = true;
        //     while (!uxManager.Project.hasTrackerMarker)
        //     {
        //         if (debug)
        //         {
        //             Debug.Log("Waiting for marker");
        //             debug = false;
        //         }
        //         await System.Threading.Tasks.Task.Delay(100);
        //     }
        //     Debug.Log("Marker found");
        //     DisplayProject();
        // }

        // #endregion Marker

        // #region Geolocation

        // void SetGeoAnchorAndDisplayProject(bool b, string s, GameObject anchor)
        // {
        //     Debug.Log("SetGeoAnchorAndDisplayProject: " + s);
        //     if (!b)
        //     {
        //         GeolocationUnsuccessful();
        //     }
        //     else
        //     {
        //         uxManager.Project.SetGeoAnchor(anchor);
        //         uxManager.KeepProjectAlignedToGeoAnchor();
        //         DisplayProject();
        //     }
        // }
        // void GeolocationUnsuccessful()
        // {
        //     uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", true);
        //     uxManager.UIManager.DisplayUI("warning-with-options", root =>
        //     {
        //         root.Q<Label>("warning").text = "Det gick inte att geolokalisera!";
        //         root.Q<Button>("option-one").clicked += () =>
        //         {
        //             uxManager.Project.overrideGeolocation = true;
        //             uxManager.GeospatialManager.GeospatialEnabled = false;
        //             uxManager.GeospatialManager.OnLocalizationUnsuccessful.RemoveAllListeners();
        //             DisplayProject();
        //         };
        //         root.Q<Button>("option-one").text = "Visa projektet ändå";
        //         root.Q<Button>("option-two").clicked += () =>
        //         {
        //             uxManager.AppManager.DisplayRecentProjectList(() => { uxManager.AppManager.LoadProjectCollections(); });
        //         };
        //         root.Q<Button>("option-two").text = "Återgå till projektmenyn";
        //     });
        // }

        // #endregion Geolocation

        // #region Display Project

        async void DisplayProject()
        {
            Debug.Log("Display project");
            try
            {
                uxManager.UIManager.ShowLoading("Skapar projekt...");
                // Waiting for the project to load 
                await uxManager.Project.CreateProject();
                DisplayProjectInfo();
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
                uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", true);
                uxManager.UIManager.DisplayUI("warning-with-options", root =>
                {
                    root.Q<Label>("warning").text = "Det gick inte att skapa projektet!";
                    root.Q<Button>("option-one").clicked += () =>
                    {
                        uxManager.AppManager.DisplayRecentProjectList(() => { uxManager.AppManager.LoadProjectCollections(); });
                    };
                    root.Q<Button>("option-one").text = "Återgå till projektmenyn";
                    root.Q<Button>("option-two").style.display = DisplayStyle.None;
                });
            }
        }
        void DisplayProjectInfo()
        {
            uxManager.UIManager.MenuManager.ToggleMenuItemInteractable("project-list", true);

            uxManager.UIManager.DisplayUI("project-info", root =>
                    {
                        root.Q<Label>("Name").text = uxManager.Project.name;
                        root.Q<Label>("Description").text = uxManager.Project.description;

                        Button start = root.Q<Button>("start");
                        if (uxManager.Project.hasLibraryResources || uxManager.Project.hasInteractiveResources)
                        {
                            start.clicked += () =>
                            {
                                uxManager.ShowWorkspaceDefault();
                            };
                        }
                        else
                        {
                            start.style.visibility = Visibility.Hidden;
                            Debug.Log($"Project has library resources {uxManager.Project.hasLibraryResources} and interactive resources {uxManager.Project.hasInteractiveResources}.");
                        }

                        Button proposals = root.Q<Button>("proposals");
                        if (uxManager.Project.hasProposals)
                        {
                            proposals.clicked += () =>
                            {
                                IUXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
                                uxManager.UseUxHandler(ux);
                            };
                        }
                        else
                        {
                            proposals.style.visibility = Visibility.Hidden;
                            Debug.Log($"Project has proposals {uxManager.Project.hasProposals}.");
                        }

                    }
            );
        }

        // #endregion Display Project

        public override void Deactivate()
        {

        }
    }
}
