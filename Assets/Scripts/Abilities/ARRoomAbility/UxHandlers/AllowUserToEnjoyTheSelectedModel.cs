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
            WorkspaceObject.UseLayers(Layers.Model);
            // SelectObject(WorkspaceObject.GameObject);
            workspace.UseHud("user-can-cancel-inspect-model-hud", root =>
            {
                root.Q<Button>("done").clicked += () => Done(workspace);
            });
        }

        public override void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Deactivate(scene, workspace);
            WorkspaceObject.UseLayers(Layers.Marker);
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return new []{ WorkspaceObject.GameObject };
        }
    }
}