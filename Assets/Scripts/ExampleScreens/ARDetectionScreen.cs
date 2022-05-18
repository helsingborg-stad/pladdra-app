using System;
using System.Collections.Generic;
using System.Linq;
using ARHandlers;
using DefaultNamespace;
using Screens;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Workspace;
using Workspace.Hud;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class ARDetectionScreen : Screen
    {
        public IARHandler ARHandler { get; set; }
        public WorkspaceConfiguration configuration { get; private set; }

        public void SetWorkspaceConfiguration(WorkspaceConfiguration wc)
        {
            configuration = wc;
        }

        public ARDetectionScreen()
        {
            ARHandler = new NullARHandler();
        }

        protected override void AfterActivateScreen()
        {
            FindObjectOfType<HudManager>().UseHud("ar-is-detecting-tracked-image", root =>
            {
                TrackImageOnFloorAndThen(go =>
                {
                    go.name = "WorkspaceOrigin";
                    configuration.Origin.go = go;
                    GetComponentInParent<ScreenManager>().SetActiveScreen<WorkspaceScreen>(
                        beforeActivate: screen => screen.SetWorkspaceConfiguration(configuration)
                    );
                });
            });
        }

        private void TrackImageOnFloorAndThen(Action<GameObject> action)
        {
            var hitsOnFloor = new List<ARRaycastHit>();
            var actions = new[]
            {
                action
            };

            UseARHandler(
                new CompositeARHandler(new IARHandler[]
                {
                    new ARPlaneDetectionHandler(),
                    new ARScreenRaycastHandler(
                        hits => { hitsOnFloor = hits; },
                        h => { hitsOnFloor = new List<ARRaycastHit>(); }),
                    new ARTrackImageHandler(trackedImagesChangedEventArgs =>
                    {
                        var trackedImagesChangedEvents = new[]
                            {
                                trackedImagesChangedEventArgs.added,
                                trackedImagesChangedEventArgs.updated,
                            }
                            .SelectMany(items => items)
                            .ToArray();

                        actions
                            .Where(a
                                => actions.Length == 1 && hitsOnFloor.Count > 0 && trackedImagesChangedEvents.Length > 0)
                            .Select(a => new
                            {
                                trackedImage = trackedImagesChangedEvents.FirstOrDefault(),
                                hit = hitsOnFloor.FirstOrDefault()
                            })
                            .Select(sources => new
                            {
                                position = new Vector3(sources.trackedImage.transform.position.x,
                                    sources.hit.pose.position.y, sources.trackedImage.transform.position.z),
                                rotation = Quaternion.Euler(0, sources.trackedImage.transform.eulerAngles.y, 0)
                            })
                            .ToList()
                            .ForEach(aggregated =>
                            {
                                actions = Array.Empty<Action<GameObject>>();
                                UseARHandler(new NullARHandler());

                                var go = new GameObject();
                                go.transform.position = aggregated.position;
                                go.transform.rotation = aggregated.rotation;

                                action(go);
                            });
                    })
                })
            );
        }

        private void UseARHandler(IARHandler arHandler)
        {
            ARHandler.Deactivate();
            ARHandler = arHandler ?? new NullARHandler();
            ARHandler.Activate();
        }
    }
}