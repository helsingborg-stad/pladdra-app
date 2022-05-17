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
using Workspace.UxHandlers;

namespace Workspace
{
    public class WorkspaceManager : MonoBehaviour, IWorkspaceScene, IWorkspace
    {
        public IUxHandler UxHandler { get; set; }
        public GameObject workspaceOrigin;
        public GameObject itemPrefab;
        public IWorkspaceObjectsManager ObjectsManager { get; set; }
        public IWorkspaceResourceCollection Resources => Configuration.ResourceCollection;

        public void UseScene(string name, DialogScene scene)
        {
            ObjectsManager.DestroyAll();

            UpdateTransform(Plane, scene?.Plane);

            foreach (var item in scene?.Items ??
                                 Enumerable.Empty<DialogScene.ItemDescription>())
            {
                var resource = Configuration.ResourceCollection.TryGetResource(item.ResourceId);
                if (resource != null)
                    ObjectsManager.SpawnItem(
                        Plane,
                        resource,
                        item.Position?.ToVector3() ?? new Vector3(1, 1, 1),
                        item.Rotation?.ToQuaternion() ?? new Quaternion(),
                        item.Scale?.ToVector3() ?? new Vector3(1, 1, 1)
                    );
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
            UxHandler.Deactivate(this, this);
            UxHandler = handler ?? new NullUxHandler();
            UxHandler.Activate(this, this);
        }

        public DialogScene CreateWorkspaceSceneDescription()
        {
            return DialogScene.Describe(this);
        }

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
            UseUxHandler(new AllowUserSelectWorkspaceActions());
        }
    }
}