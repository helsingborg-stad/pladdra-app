using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Dialogs;
using DefaultNamespace;
using Newtonsoft.Json;
using Pipelines;
using Repository;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace
{
    public class WorkspaceManager : MonoBehaviour, IWorkspaceManager, IWorkspaceScene
    {
        public IUxHandler UxHandler { get; set; }
        public GameObject workspaceOrigin;
        public GameObject itemPrefab;
        public IWorkspaceObjectsManager ObjectsManager { get; set; }
        public IWorkspaceResourceCollection Resources => Configuration.ResourceCollection;

        public void UseScene(string name, DialogScene scene)
        {
            UpdateTransform(Plane, scene?.Plane);

            ObjectsManager.DestroyAll();
            
            foreach (var item in scene?.Items ??
                                 Enumerable.Empty<DialogScene.ItemDescription>())
            {
                var resource = Configuration.ResourceCollection.TryGetResource(item.ResourceId);
                if (resource != null)
                {
                    ObjectsManager.SpawnItem(
                        Plane,
                        resource,
                        item.Position?.ToVector3() ?? new Vector3(1, 1, 1),
                        item.Rotation?.ToQuaternion() ?? new Quaternion(),
                        item.Scale?.ToVector3() ?? new Vector3(1, 1, 1)
                    );
                } 
            }

            void UpdateTransform(GameObject go, DialogScene.TransformDescription t)
            {
                go.transform.localPosition = t?.Position?.ToVector3() ?? go.transform.localPosition;
                go.transform.localScale = t?.Scale?.ToVector3() ?? go.transform.localScale;
                go.transform.localRotation = t?.Rotation?.ToQuaternion() ?? go.transform.localRotation;
            }
        }

        public void UseHud(string templatePath, Action<VisualElement> bindUi)
        {
            FindObjectOfType<HudManager>()
                .UseHud(templatePath, bindUi);
        }

        public void UseUxHandler(IUxHandler handler)
        {
            Debug.Log(JsonConvert.SerializeObject(
                DialogScene.Describe(this),
                Formatting.Indented
                ));
            UxHandler.Deactivate(this);
            UxHandler = handler ?? new NullUxHandler();
            UxHandler.Activate(this);
        }

        public DialogScene CreateWorkspaceSceneDescription() => DialogScene.Describe(this);
        public void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then)
        {
            ClearHud();
            StartCoroutine(CR());
            IEnumerator CR()
            {
                yield return new TaskYieldInstruction<T>(waitFor, then);
            }
        }

        public void ClearHud()
        {
            FindObjectOfType<HudManager>().ClearHud();
        }

        private WorkspaceConfiguration Configuration { get; set; }

        public IDialogProjectRepository DialogProjectRepository => FindObjectOfType<RepositoryManager>();
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

            UseScene("", wc.Scene);
 
            /*
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
            */
            UseUxHandler(new AllowUserToPositionObjects());
//            UseUxHandler(new AllowUserToPositionPlane());
//            SetModeAllowUserToPositionPlane();
        }
    }
}