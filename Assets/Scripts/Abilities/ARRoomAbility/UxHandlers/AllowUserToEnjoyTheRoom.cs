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
        private List<DialogScene> FeaturedScenes { get; }
        private GameObject HighlightedModel { get; set; }

        public AllowUserToEnjoyTheRoom(List<DialogScene> featuredScenes)
        {
            FeaturedScenes = featuredScenes;
        }

        protected override void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            base.OnSelected(scene, workspace, go);

            var selected = scene.ObjectsManager.Objects.FirstOrDefault(obj => obj.GameObject == go);
            if (selected != null)
            {
                var children = selected?.ChildGameObjects;
                workspace.UseUxHandler(new AllowUserToEnjoyTheSelectedModel(selected));
            }
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
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
                            workspace.UseScene(scene);
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