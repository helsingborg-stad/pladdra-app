using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UXHandlers;

namespace Pladdra.Workspace.UxHandlers
{
    public class AllowUserSelectWorkspaceActions: AbstractUxHandler
    {
        public override void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            base.Activate(scene, workspace);
            //TODO: Find solution for handling PlaneMesh 
            UnityEngine.GameObject.Find("PlaneMesh").GetComponent<MeshRenderer>().enabled = true;
            workspace.UseHud("user-can-chose-workspace-action-hud", root =>
            {
                root.Q<Label>("workspace-name").text = workspace.Name;

                var previewButton = root.Q<Button>("preview");
                previewButton.clicked += () => workspace.Actions.DispatchAction("preview");
                previewButton.visible = workspace.Actions.HasAction("preview");

                root.Q<Button>("edit-plane").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToPositionPlane());
                };
                root.Q<Button>("edit-objects").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToPositionObjects());
                };
                root.Q<Button>("inventory").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToSpawnItemFromResource());
                };
                root.Q<Button>("load-scene").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToLoadWorkspaceScene());
                };
                root.Q<Button>("save-scene").clicked += () =>
                {
                    workspace.UseUxHandler(new AllowUserToSaveWorkspaceScene());
                };
                
                var undoButton = root.Q<Button>("history-undo");
                undoButton.SetEnabled(workspace.HistoryActions.CanUndo);
                undoButton.clicked += () => workspace.HistoryActions.Undo();
                
                var redoButton = root.Q<Button>("history-redo");
                redoButton.SetEnabled(workspace.HistoryActions.CanRedo);
                redoButton.clicked += () => workspace.HistoryActions.Redo();
            });
        }

        protected override IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene)
        {
            return Enumerable.Empty<GameObject>();
        }
    }
}