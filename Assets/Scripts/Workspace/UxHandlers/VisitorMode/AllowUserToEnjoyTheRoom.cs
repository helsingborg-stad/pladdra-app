using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers.VisitorMode
{
    public class AllowUserToEnjoyTheRoom: AbstractUxHandler
    {
        private List<DialogScene> FeaturedScenes { get; }

        public AllowUserToEnjoyTheRoom(List<DialogScene> featuredScenes)
        {
            FeaturedScenes = featuredScenes;
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
            return Enumerable.Empty<GameObject>();
        }
    }
}