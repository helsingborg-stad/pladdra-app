using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace;

namespace Abilities.ARRoomAbility.UxHandlers
{
    public class AllowUserToEnjoyTheSelectedModel : AbstractUxHandler
    {
        private IWorkspaceObject WorkspaceObject { get; }
        private Action<IWorkspace> Done { get; }

        public AllowUserToEnjoyTheSelectedModel(IWorkspaceObject workspaceObject, Action<IWorkspace> onDone): base(Traits.AllowBoxCollider, Traits.AllowFlexibleBounds, Traits.AllowScale)
        {
            WorkspaceObject = workspaceObject;
            Done = onDone;
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            scene.ObjectsManager.Objects.Where(o => WorkspaceObject != o).ToList().ForEach(o => o.GameObject.SetActive(false));
            
            WorkspaceObject.ChildGameObjects.Select((go, index) =>
            {
                go.SetActive(index == 1);
                return 0;
            }).ToList();

            workspace.UseHud("user-can-cancel-inspect-model-hud", root =>
            {
                root.Q<Button>("done").clicked += () => Done(workspace);
            });
        }

        public override void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Deactivate(scene, workspace);
            scene.ObjectsManager.Objects.Where(o => WorkspaceObject != o).ToList().ForEach(o => o.GameObject.SetActive(true));
            WorkspaceObject.ChildGameObjects.Select((go, index) =>
            {
                go.SetActive(index == 0);
                return 0;
            }).ToList();
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return new []{ WorkspaceObject.GameObject };
        }
    }
}