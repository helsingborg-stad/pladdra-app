using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UXHandlers;
using Workspace;

namespace Abilities.ARRoomAbility.UxHandlers
{
    public class AllowUserToEnjoyTheSelectedModel : AbstractUxHandler
    {
        public IWorkspaceObject WorkspaceObject { get; }

        public AllowUserToEnjoyTheSelectedModel(IWorkspaceObject workspaceObject)
        {
            WorkspaceObject = workspaceObject;
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            WorkspaceObject.ChildGameObjects.Select((go, index) =>
            {
                go.SetActive(index == 1);
                return 0;
            }).ToList();
        }

        public override void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Deactivate(scene, workspace);
            WorkspaceObject.ChildGameObjects.Select((go, index) =>
            {
                go.SetActive(index == 0);
                return 0;
            }).ToList();
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return WorkspaceObject.ChildGameObjects;
        }
    }
}