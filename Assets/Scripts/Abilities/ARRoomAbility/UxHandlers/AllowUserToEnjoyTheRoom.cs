using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace;

namespace Abilities.ARRoomAbility.UxHandlers
{
    public class AllowUserToEnjoyTheRoom: AbstractUxHandler
    {
        private IRaycastHandler RaycastHandler { get; set; }
        private DialogScene FeaturedScene { get; }
        private List<DialogScene> FeaturedScenes { get; }

        public AllowUserToEnjoyTheRoom(DialogScene featuredScene, List<DialogScene> featuredScenes) : base(Traits.AllowBoxCollider, Traits.AllowFlexibleBounds, Traits.AllowSelect)
        {
            FeaturedScene = featuredScene;
            FeaturedScenes = featuredScenes;
            RaycastHandler = RaycastHandler ?? new NullRaycastHandler();
        }

        public override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnSelected(scene, workspace, go);
            UseRaycast(new NullRaycastHandler());
            
            var selected = scene.ObjectsManager.Objects.FirstOrDefault(obj => obj.ContainsGameObject(go));
            if (selected != null)
            {
                workspace.UseUxHandler(new AllowUserToEnjoyTheSelectedModel(selected, ws =>
                {
                    ws.UseUxHandler(new AllowUserToEnjoyTheRoom(FeaturedScene, FeaturedScenes));
                }));
            }
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            // render the scene before attaching events
            workspace.UseScene(FeaturedScene);
            base.Activate(scene, workspace);
            workspace.ClearHud();
            
            //TODO: Find solution for handling PlaneMesh
            UnityEngine.GameObject.Find("PlaneMesh").GetComponent<MeshRenderer>().enabled = false;
            //UnityEngine.GameObject.Find("LeanSelect").GetComponent<LeanFingerDown>().enabled = false;

            GameObject focusedGo = null;
            
            workspace.UseHud("user-can-enjoy-the-room-hud", root =>
            {
                root.Q<Button>("inspect-model-button").clicked += () => SelectObject((focusedGo));
                
                UseRaycast(new RaycastSelectables((successHit, hitGo) =>
                {
                    root.Q<VisualElement>("crosshair_inner").style.backgroundColor =
                        successHit ? Color.white : Color.clear;
                    root.Q<Button>("inspect-model-button").style.display = successHit ? DisplayStyle.Flex : DisplayStyle.None;
                    focusedGo = successHit ? hitGo : null;
                }));

                if (FeaturedScenes?.Count > 1)
                {
                    var current = FeaturedScenes.FindIndex(scene => scene.Name == FeaturedScene.Name);
                    root.Q<Button>("previous-scene-button").clicked += () =>
                    {
                        workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(FeaturedScenes[current > 0 ? current - 1 : FeaturedScenes.Count - 1], FeaturedScenes));
                    };
                    root.Q<Button>("next-scene-button").clicked += () =>
                    {
                        workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(FeaturedScenes[FeaturedScenes.Count - 1 > current ? current + 1 : 0], FeaturedScenes));
                    };
                    
                    root.Q<Label>("scene-name").text = FeaturedScene.Name;
                    root.Q<VisualElement>("scene-navigation").style.display = DisplayStyle.Flex;
                }
                
                if (workspace.Actions.HasAction("cancel-preview"))
                {
                    root.Q<Button>("exit-preview-button").clicked += () => workspace.Actions.DispatchAction("cancel-preview");
                    root.Q<Button>("exit-preview-button").style.display = DisplayStyle.Flex;
                }
            });
        }

        public override void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            UseRaycast(new NullRaycastHandler());
            base.Deactivate(scene, workspace);
            
            GameObject.Find("LeanSelect").GetComponent<LeanFingerDown>().enabled = true;
        }

        private void UseRaycast(IRaycastHandler handler)
        {
            // Todo: Move out to RaycastManager
            RaycastHandler.Deactivate();
            RaycastHandler = handler ?? new NullRaycastHandler();
            RaycastHandler.Activate();
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return from o in scene.ObjectsManager.Objects
                from go in o.LayerObjects.Values
                select go;
        }
        
    }
}