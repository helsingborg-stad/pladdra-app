using System;
using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace;

namespace Abilities.ARRoomAbility.UxHandlers
{
    public class AllowUserToEnjoyTheRoom: AbstractUxHandler
    {
        private DialogScene FeaturedScene { get; }
        private List<DialogScene> FeaturedScenes { get; }

        public AllowUserToEnjoyTheRoom(DialogScene featuredScene, List<DialogScene> featuredScenes) : base(Traits.AllowBoxCollider, Traits.AllowFlexibleBounds, Traits.AllowSelect, Traits.AllowOutlineSelected)
        {
            FeaturedScene = featuredScene;
            FeaturedScenes = featuredScenes;
        }

        public override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnSelected(scene, workspace, go);

            var selected = scene.ObjectsManager.Objects.FirstOrDefault(obj => obj.GameObject == go);
            if (selected != null)
            {
                var children = selected?.ChildGameObjects;
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
            
            if (workspace.Actions.HasAction("cancel-preview"))
            {
                workspace.UseHud("user-can-cancel-preview-mode-hud", root =>
                {
                    root.Q<Button>("done").clicked += () => workspace.Actions.DispatchAction("cancel-preview");
                });
                return;
            }
            
            if (FeaturedScenes?.Count > 1)
            {
                workspace.UseHud("user-can-choose-between-featured-scenes-hud", root =>
                {
                    var container = root.Q<VisualElement>("content");
                    FeaturedScenes.ForEach(scene =>
                    {
                        var button = new Button
                        {
                            text = scene.Name
                        };
                        button.clicked += () =>
                        {
                            workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(scene, FeaturedScenes));
                        };
                        container.Add(button);
                    });
                });
            }
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return scene.ObjectsManager.Objects.Select(o => o.GameObject);
        }
    }
}