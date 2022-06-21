using System.Collections.Generic;
using System.Linq;
using Abilities.ARRoomAbility.UxHandlers;
using Data.Dialogs;
using Workspace;
using Workspace.UxHandlers;

namespace Abilities.ARRoomAbility
{
    public class ArDialogueRoomAbility : IAbility
    {
        public IDialogProjectRepository Repository { get; }
        public bool IsEditMode { get; set; }

        public void ConfigureWorkspace(WorkspaceConfiguration configuration, IWorkspace workspace)
        {
            if (IsEditMode)
            {
                workspace.Actions.RegisterAction("default", (scene, workspace) =>
                {
                    workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
                    workspace.UseLayers(Layers.Marker);
                });
                workspace.Actions.RegisterAction("cancel-preview", (scene, workspace) => workspace.Actions.DispatchAction("default"));
                workspace.Actions.RegisterAction("preview", (scene, workspace) =>
                {
                    var s = workspace.GetSceneDescription();
                    workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(
                        s, new List<DialogScene>() { s }));
                });
            }
            else
            {
                workspace.Actions.RegisterAction("default", (scene, workspace) => workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(configuration.FeaturedScenes.FirstOrDefault(), configuration.FeaturedScenes)));
            }
            workspace.Actions.DispatchAction("default");
        }

        public ArDialogueRoomAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }
    }
}