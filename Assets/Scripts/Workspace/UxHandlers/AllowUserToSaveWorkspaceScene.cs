using System.Collections.Generic;
using System.Linq;
using Repository;
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

        public override void Activate(IWorkspaceScene scene)
        {
            base.Activate(scene);
            scene.UseHud("user-can-save-workspace-hud", root =>
            {
                var saveButton = root.Q<Button>("save");
                var cancelButton = root.Q<Button>("cancel");
                var nameInput = root.Q<TextField>("name");
                saveButton.clicked += () =>
                {
                    // extract stuff in man thread
                    var repo = scene.DialogProjectRepository;
                    var sceneDescription = scene.CreateWorkspaceSceneDescription();
                    // ...and then save in worker
                    scene.WaitForThen(
                        () => repo.SaveScene(nameInput.value,
                            sceneDescription),
                        (_) => scene.UseUxHandler(new AllowUserToPositionObjects())
                    );
                };
                cancelButton.clicked += () =>
                {
                    scene.UseUxHandler(new AllowUserToPositionObjects());
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