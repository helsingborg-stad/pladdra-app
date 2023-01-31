using System;
using System.Collections.Generic;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToViewProject : UXHandler
    {
        public AllowUserToViewProject(UXManager uxManager)
        {
            this.uxManager = uxManager;
        }
        public override void Activate()
        {
            Debug.Log("Init Project " + uxManager.Project.name);

            if (uxManager.Project.requiresGeolocation)
            {
                uxManager.UIManager.DisplayUI("prompt", root =>
                {
                    root.Q<Label>("prompt").text = "Titta dig omkring för att lokalisera AR.";
                    root.Q<Button>("option-one").clicked += () =>
                    {
                        uxManager.Project.overrideGeolocation = true;
                        DisplayProject();
                    };
                    root.Q<Button>("option-one").text = "Skippa geolokalisering";
                });
                Debug.Log("Project requires location to be displayed.");
                Action<bool, string, GameObject> placedObject = SetGeoAnchorAndDisplayProject;
                uxManager.GeospatialManager.OnLocalizationUnsuccessful.AddListener(GeolocationUnsuccessful);
                uxManager.GeospatialManager.PlaceGeoAnchorAtLocation("id", uxManager.Project.location.Item1, uxManager.Project.location.Item2, Quaternion.identity, placedObject);
            }
            else if (uxManager.Project.marker.required)
            {
                Debug.Log("Project requires marker to be displayed.");
                uxManager.UIManager.DisplayUI("look-for-marker");
                // TODO Hook up listeners to marker found event
            }
            else
            {
                DisplayProject();
            }
        }

        void SetGeoAnchorAndDisplayProject(bool b, string s, GameObject anchor)
        {
            Debug.Log("SetGeoAnchorAndDisplayProject: " + s);
            if (!b)
            {
                GeolocationUnsuccessful();

                //TODO TESTING
                // uxManager.Project.SetGeoAnchor(UnityEngine.GameObject.Find("Capsule"));
                // DisplayProject();
            }
            else
            {
                uxManager.Project.SetGeoAnchor(anchor);
                // uxManager.UIManager.MenuManager.AddMenuItem(new MenuItem()
                // {
                //     id = "alignToGeoAnchor",
                //     name = "Geolocalisera",
                //     action = () =>
                //     {
                //         uxManager.Project.AlignToGeoAnchor();
                //     }
                // });
                DisplayProject();
            }
        }
        void GeolocationUnsuccessful()
        {
            uxManager.UIManager.DisplayUI("warning-with-options", root =>
            {
                root.Q<Label>("warning").text = "Det gick inte att geolokalisera!";
                root.Q<Button>("option-one").clicked += () =>
                {
                    uxManager.Project.overrideGeolocation = true;
                    DisplayProject();
                };
                root.Q<Button>("option-one").text = "Visa projektet ändå";
                root.Q<Button>("option-two").clicked += () =>
                {
                    uxManager.AppManager.DisplayRecentProjectList(() => { uxManager.AppManager.LoadProjectCollections(); });
                };
                root.Q<Button>("option-two").text = "Återgå till projektmenyn";
            });
        }

        async void DisplayProject()
        {
            Debug.Log("Display project");
            uxManager.UIManager.ShowLoading("Skapar projekt...");
            await uxManager.Project.CreateProject();
            DisplayProjectInfo();
        }
        void DisplayProjectInfo()
        {
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
                                UXHandler ux = new AllowUserToViewProposalLibrary(uxManager);
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

        public override void Deactivate()
        {

        }
    }
}
