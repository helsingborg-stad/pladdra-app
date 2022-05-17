using System;
using System.Collections.Generic;
using System.Linq;
using ARHandlers;
using DefaultNamespace;
using Screens;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Workspace;
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
            var hits = new List<ARRaycastHit>();
            var actions = new[]
            {
                action
            };

            UseARHandler(
                new CompositeARHandler(new IARHandler[]
                {
                    new ARPlaneDetectionHandler(),
                    new ARScreenRaycastHandler(
                        successHits => { hits = successHits; },
                        h => { hits = new List<ARRaycastHit>(); }),
                    new ARTrackImageHandler(trackedImageEvent =>
                    {
                        var trackedImages = new[]
                            {
                                trackedImageEvent.added,
                                trackedImageEvent.updated,
                            }
                            .SelectMany(items => items)
                            .ToArray();

                        actions
                            .Where(a => actions.Length == 1 && hits.Count > 0 && trackedImages.Length > 0)
                            .Select(a => new
                            {
                                trackedImage = trackedImages.FirstOrDefault(),
                                hit = hits.FirstOrDefault(),
                                callback = a
                            })
                            .Where(obj => (obj.trackedImage != null))
                            .ToList()
                            .ForEach(obj =>
                            {
                                actions = Array.Empty<Action<GameObject>>();

                                var go = new GameObject();

                                go.transform.position = new Vector3(obj.trackedImage.transform.position.x,
                                    obj.hit.pose.position.y, obj.trackedImage.transform.position.z);
                                go.transform.rotation =
                                    Quaternion.Euler(0, obj.trackedImage.transform.eulerAngles.y, 0);

                                UseARHandler(new NullARHandler());

                                obj.callback(go);
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