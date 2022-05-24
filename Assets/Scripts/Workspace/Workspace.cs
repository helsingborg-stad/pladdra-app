using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abilities.ARRoomAbility;
using Data.Dialogs;
using Pipelines;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;
using Workspace.EditHistory;
using Workspace.Hud;

namespace Workspace
{
    public class Workspace: IWorkspace {
        public Workspace(MonoBehaviour owner, IWorkspaceScene scene, IWorkspaceObjectsManager objectsManager, IWorkspaceResourceCollection resourceCollection, IHudManager hudManager, IDialogProjectRepository dialogProjectRepository, IWorkspaceEditHistory history)
        {
            Owner = owner;
            Scene = scene;
            ObjectsManager = objectsManager;
            ResourceCollection = resourceCollection;
            HudManager = hudManager;
            DialogProjectRepository = dialogProjectRepository;
            History = history;
            UxHandler = new NullUxHandler();
            HistoryActions = new WorkspaceEditHistoryActions(History, scene =>
            {
                UseScene(scene);
                UseUxHandler(UxHandler, false);
            });
        }

        private MonoBehaviour Owner { get; set; }
        private IUxHandler UxHandler { get; set; }
        private IWorkspaceScene Scene { get; set;  }
        private IWorkspaceObjectsManager ObjectsManager { get; set; }
        private IWorkspaceResourceCollection ResourceCollection { get; set; }
        private IHudManager HudManager { get; set; }
        private IWorkspaceEditHistory History { get;  set;  }
        public string Name { get; private set; }
        public IEnumerable<DialogScene> FeaturedScenes { get; private set;  }
        public IWorkspaceEditHistoryActions HistoryActions { get; }
        public IDialogProjectRepository DialogProjectRepository { get; }

        public DialogScene GetSceneDescription()
        {
            return Scene.CreateWorkspaceSceneDescription(Name);
        }

        public void UseScene(DialogScene scene)
        {
            ObjectsManager.DestroyAll();

            UpdateTransform(Scene.Plane, scene?.Plane);

            foreach (var item in scene?.Items ??
                                 Enumerable.Empty<DialogScene.ItemDescription>())
            {
                var resource = ResourceCollection.TryGetResource(item.ResourceId);
                if (resource != null)
                    ObjectsManager.SpawnItem(resource, Scene.Plane, item.Position?.ToVector3() ?? new Vector3(1, 1, 1), item.Rotation?.ToQuaternion() ?? new Quaternion(), item.Scale?.ToVector3() ?? new Vector3(1, 1, 1));
            }

            void UpdateTransform(GameObject go, DialogScene.TransformDescription t)
            {
                go.transform.localPosition = t?.Position?.ToVector3() ?? go.transform.localPosition;
                go.transform.localScale = t?.Scale?.ToVector3() ?? go.transform.localScale;
                go.transform.localRotation = t?.Rotation?.ToQuaternion() ?? go.transform.localRotation;
            }

            Name = scene?.Name ?? "";
        }

        public void UseHud(string templatePath, Action<VisualElement> bindUi)
        {
            HudManager.UseHud(templatePath, bindUi);
        }

        public void UseUxHandler(IUxHandler handler)
        {
            UseUxHandler(handler, true);
        }
        private void UseUxHandler(IUxHandler handler, bool updateHistory)
        {
            if (updateHistory)
            {
                History.SaveSnapshot(this);
            }
            UxHandler.Deactivate(Scene, this);
            UxHandler = handler ?? new NullUxHandler();
            UxHandler.Activate(Scene, this);
        }

        public void WaitForThen<T>(Func<Task<T>> waitFor, Action<T> then)
        {
            UseHud("app-is-busy-hud", root => {});
            Owner.StartCoroutine(CR());
            IEnumerator CR()
            {
                yield return new TaskYieldInstruction<T>(waitFor, result =>
                {
                    ClearHud();
                    then(result);
                });
            }
        }

        public void ClearHud()
        {
            HudManager.ClearHud();
        }
    }
}