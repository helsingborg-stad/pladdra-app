using System.Collections.Generic;
using Abilities.ARRoomAbility;
using Abilities.ARRoomAbility.UxHandlers;
using Data.Dialogs;
using Workspace;
using Workspace.UxHandlers;

namespace Abilities.Tutorial
{

    public class TutorialAbility : IAbility
    {
        public IDialogProjectRepository Repository { get; }

        public TutorialAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }

        public void ConfigureWorkspace(WorkspaceConfiguration configuration, IWorkspace workspace)
        {
            // The workspace assumes some well known actions to be defined
            // We implement these by going directly into edit mode (as defined the ARRoomAbility)
            workspace.Actions.RegisterAction("default", (scene, workspace) => workspace.UseUxHandler(new AllowUserSelectWorkspaceActions()));
            workspace.Actions.RegisterAction("cancel-preview", (scene, workspace) => workspace.Actions.DispatchAction("default"));
            workspace.Actions.RegisterAction("preview", (scene, workspace) =>
            {
                var s = workspace.GetSceneDescription();
                workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(
                    s, new List<DialogScene>() { s }));
            });

            // start workspace...
            workspace.Actions.DispatchAction("default");
        }
    }
}