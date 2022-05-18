using Repository;
using UnityEngine;
using Workspace.Hud;
using Workspace.UxHandlers;

namespace Workspace
{
    public class WorkspaceManager : MonoBehaviour
    {
        private IWorkspaceScene Scene { get; set; }
        private IWorkspace Workspace { get; set; }
        
        
        // public IUxHandler UxHandler { get; set; }
        public GameObject workspaceOrigin;
        public GameObject itemPrefab;

        public void Activate(WorkspaceConfiguration wc)
        {
            var hudManager = FindObjectOfType<HudManager>();
            var dialogProjectRepository = FindObjectOfType<RepositoryManager>();
            var objectsManager = new WorkspaceObjectsManager(itemPrefab);
            
            workspaceOrigin.transform.position = wc.Origin.Position;
            workspaceOrigin.transform.rotation = wc.Origin.Rotation;
            // TODO: Destroy/detach any existing children of workspaceOrigin
            
            var plane = FindObjectOfType<PlaneFactory>()
                .SpawnPlane(wc.Plane.Width, wc.Plane.Height);
            plane.transform.SetParent(workspaceOrigin.transform);

            Scene = new WorkspaceScene(plane, objectsManager, wc.ResourceCollection);
            
            Workspace = new Workspace(this, Scene, objectsManager, wc.ResourceCollection, hudManager, dialogProjectRepository);
            
            Workspace.UseScene(wc.Scene);
            Workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
        }
    }
}