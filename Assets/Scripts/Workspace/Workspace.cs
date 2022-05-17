using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Data.Dialogs;
using Pipelines;
using Repository;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.Hud;

namespace Workspace
{
    public class Workspace: IWorkspace {
        public Workspace(MonoBehaviour owner, IWorkspaceScene scene, IWorkspaceObjectsManager objectsManager, IWorkspaceResourceCollection resourceCollection, IHudManager hudManager, IDialogProjectRepository dialogProjectRepository)
        {
            Owner = owner;
            Scene = scene;
            ObjectsManager = objectsManager;
            ResourceCollection = resourceCollection;
            HudManager = hudManager;
            DialogProjectRepository = dialogProjectRepository;
            UxHandler = new NullUxHandler();
        }

        private MonoBehaviour Owner { get; set; }
        private IUxHandler UxHandler { get; set; }
        private IWorkspaceScene Scene { get; set;  }
        private IWorkspaceObjectsManager ObjectsManager { get; set; }
        private IWorkspaceResourceCollection ResourceCollection { get; set; }
        private IHudManager HudManager { get; set; }

        public IDialogProjectRepository DialogProjectRepository { get; }

        public void UseScene(string name, DialogScene scene)
        {
            ObjectsManager.DestroyAll();

            UpdateTransform(Scene.Plane, scene?.Plane);

            foreach (var item in scene?.Items ??
                                 Enumerable.Empty<DialogScene.ItemDescription>())
            {
                var resource = ResourceCollection.TryGetResource(item.ResourceId);
                if (resource != null)
                    ObjectsManager.SpawnItem(
                        Scene.Plane,
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
            HudManager.UseHud(templatePath, bindUi);
        }

        public void UseUxHandler(IUxHandler handler)
        {
            UxHandler.Deactivate(Scene, this);
            UxHandler = handler ?? new NullUxHandler();
            UxHandler.Activate(Scene, this);
        }

        public void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then)
        {
            ClearHud();
            Owner.StartCoroutine(CR());
            IEnumerator CR()
            {
                yield return new TaskYieldInstruction<T>(waitFor, then);
            }
        }

        public void ClearHud()
        {
            HudManager.ClearHud();
        }
    }
}