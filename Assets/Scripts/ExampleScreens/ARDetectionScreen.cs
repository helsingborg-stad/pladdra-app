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

        protected override void BeforeActivateScreen()
        {
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
                    new ARScreenRaycastHandler(successHits => { hits = successHits; },
                        h => { hits = new List<ARRaycastHit>(); }),
                    new ARTrackImage(trackedImageEvent =>
                    {
                        actions
                            .ToList()
                            .Where(a => hits.Count > 0)
                            .ToList()
                            .ForEach(a =>
                            {
                                actions = Array.Empty<Action<GameObject>>();
                                var go = new GameObject(); 
                                // TODO: Aggregate TrackedImage & RaycastHit transforms into GO
                                // ........
                                // FROM OLD CODE:
                                //  planner.fixedSpace.transform.position = new Vector3(planner.context.ar.trackedImagePosition.x, planner.context.ar.raycastHitPosition.Y, planner.context.ar.trackedImagePosition.z);
                                //  Vector3 trackedImageEuler = planner.context.ar.trackedImageRotation.eulerAngles;
                                //  Quaternion trackedImageRotation = Quaternion.Euler(0, trackedImageEuler.y, 0);
                                // ........
                                UseARHandler(new NullARHandler());
                                a(go);
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