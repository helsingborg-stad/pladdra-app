using System;
using ARHandlers;
using DefaultNamespace;
using Screens;
using UnityEngine;
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
            var raycastHit;
            var trackedImage;
            var foundImage = false;

            UseARHandler(
                new CompositeARHandler(new IARHandler[]
                {
                    new ARPlaneDetectionHandler(planeEvent =>
                    {
                        
                    }),
                    new ARRaycastHandler(raycastUpdatedEventArgs =>
                    {
                        
                        Debug.Log(raycastUpdatedEventArgs.raycast.plane);
                    }),
                    new ARTrackImage(trackedImageEvent =>
                    {
                        if (raycastHit && !trackedImage)
                        {
                            trackedImage = trackedImageEvent;
                            var go = new GameObject();
                            go.transform.position = new Vector3(trackedImage.position.x, raycastHit.position.y, trackedImage.position.z);
                            go.transform.rotation = Quaternion.Euler(0, trackedImage.rotation.eulerAngles.y, 0);
                            UseARHandler(new NullARHandler());
                            action(go);
                            
                            // FROM OLD CODE:
                            //planner.fixedSpace.transform.position = new Vector3(planner.context.ar.trackedImagePosition.x, planner.context.ar.raycastHitPosition.Y, planner.context.ar.trackedImagePosition.z);
                            //Vector3 trackedImageEuler = planner.context.ar.trackedImageRotation.eulerAngles;
                            //Quaternion trackedImageRotation = Quaternion.Euler(0, trackedImageEuler.y, 0);
                            // ........
                        }
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