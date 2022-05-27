using Abilities;
using UnityEngine;
using Workspace.EditHistory;
using Workspace.Hud;

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
        }

        public void Deactivate()
        {
            Workspace?.ClearHud();
            Workspace?.UseScene(null);
            Workspace?.UseUxHandler(null);
            if (Scene?.Plane != null)
            {
                Object.Destroy(Scene?.Plane);
            }

            Workspace = null;
            Scene = null;
        }
    }
}