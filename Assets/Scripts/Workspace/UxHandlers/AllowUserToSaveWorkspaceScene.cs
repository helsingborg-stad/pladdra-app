using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Workspace.UxHandlers
{
    public class AllowUserToSaveWorkspaceScene: AbstractUxHandler
    {
        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }

        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            workspace.UseHud("user-can-save-workspace-hud", root =>
            {
                var saveButton = root.Q<Button>("save");
                var cancelButton = root.Q<Button>("cancel");
                var nameInput = root.Q<TextField>("name");
                nameInput.value = workspace.Name;
                saveButton.clicked += () =>
                {
                    // extract stuff in man thread
                    var repo = workspace.DialogProjectRepository;
                    var sceneDescription = scene.CreateWorkspaceSceneDescription(nameInput.value);
                    // ...and then save in worker
                    workspace.WaitForThen(
                        () => repo.SaveScene(sceneDescription),
                        (_) =>
                        {
                            workspace.UseScene(sceneDescription);
                            workspace.Actions.DispatchAction("default");
                        });
                };
                cancelButton.clicked += () =>
                {
                    workspace.Actions.DispatchAction("default");
                };

                nameInput.RegisterValueChangedCallback(e => ToggleSaveButton(e.newValue));
                ToggleSaveButton(nameInput.value);

                void ToggleSaveButton(string name)
                {
                    saveButton.SetEnabled(name.Trim().Length > 0);
                } 
            });
        }
    }
}