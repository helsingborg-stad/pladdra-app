using System;
using System.Linq;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.Snapshot;

namespace Workspace
{
    public class WorkspaceManager : MonoBehaviour, IWorkspaceManager, IWorkspaceScene
    {
        public IUxHandler UxHandler { get; set; }
        public GameObject workspaceOrigin;
        public GameObject itemPrefab;
        public IWorkspaceObjectsManager ObjectsManager { get; set; }
        public IWorkspaceResourceCollection Resources => Configuration.ResourceCollection;

        public void UseHud(string templatePath, Action<VisualElement> bindUi)
        {
            FindObjectOfType<HudManager>()
                .UseHud(templatePath, bindUi);
        }

        public void UseUxHandler(IUxHandler handler)
        {
            Debug.Log(JsonConvert.SerializeObject(
                WorkspaceSceneDescription.Describe(this),
                Formatting.Indented
                ));
            UxHandler.Deactivate(this);
            UxHandler = handler ?? new NullUxHandler();
            UxHandler.Activate(this);
        }

        public void ClearHud()
        {
            FindObjectOfType<HudManager>().ClearHud();
        }

        private WorkspaceConfiguration Configuration { get; set; }

        public GameObject Plane { get; set; }

        public WorkspaceManager()
        {
            UxHandler = new NullUxHandler();
        }

        private void Awake()
        {
            ObjectsManager = new WorkspaceObjectsManager(itemPrefab);
        }

        public void Activate(WorkspaceConfiguration wc)
        {
            Configuration = wc;

            workspaceOrigin.transform.position = wc.Origin.Position;
            workspaceOrigin.transform.rotation = wc.Origin.Rotation;

            Plane = FindObjectOfType<PlaneFactory>()
                .SpawnPlane(Configuration.Plane.Width, Configuration.Plane.Height);

            Plane.transform.SetParent(workspaceOrigin.transform);

            var spawns = Configuration.Cosmos.SpaceItems
                .Select(ci => new
                {
                    resource = Configuration.ResourceCollection.TryGetResource(ci.ResourceId),
                    ci
                })
                .Where(o => o.resource != null);

            foreach (var spawn in spawns)
            {
                ObjectsManager.SpawnItem(
                    Plane,
                    spawn.resource,
                    spawn.ci.Position,
                    spawn.ci.Rotation,
                    spawn.ci.Scale);
            }
            UseUxHandler(new AllowUserToPositionObjects());
//            UseUxHandler(new AllowUserToPositionPlane());
//            SetModeAllowUserToPositionPlane();
        }
    }
}