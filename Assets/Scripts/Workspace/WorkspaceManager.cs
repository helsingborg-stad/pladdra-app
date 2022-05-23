using Abilities;
using UnityEngine;
using Workspace.EditHistory;
using Workspace.Hud;
using Workspace.UxHandlers;

namespace Workspace
{
    public class WorkspaceManager : MonoBehaviour
    {
        private IWorkspaceEditHistory History { get; set; }
        private IAbility Ability { get; set; }
        private IWorkspaceScene Scene { get; set; }
        private IWorkspace Workspace { get; set; }
        
        public GameObject itemPrefab;

        public void Activate(IAbility ability, WorkspaceConfiguration wc)
        {
            History = new WorkspaceEditHistory();
            Ability = ability;
            var hudManager = FindObjectOfType<HudManager>();
            var dialogProjectRepository = Ability.Repository;
            var objectsManager = new WorkspaceObjectsManager(itemPrefab);
            
            // TODO: Destroy/detach any existing children of workspaceOrigin
            
            var plane = FindObjectOfType<PlaneFactory>()
                .SpawnPlane(wc.Plane.Width, wc.Plane.Height);
            
            plane.transform.SetParent(wc.Origin.go.transform, false);

            Scene = new WorkspaceScene(plane, objectsManager, wc.ResourceCollection);

            Workspace = new Workspace(this, Scene, objectsManager, wc.ResourceCollection, hudManager,
                dialogProjectRepository, History);

            Ability.ConfigureWorkspace(wc, Workspace);
            // Workspace.UseScene(wc.Scene);
            // Workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
        }
    }
}