using System.Collections.Generic;
using System.Linq;
using Abilities.ARRoomAbility.UxHandlers;
using Data.Dialogs;
using Pladdra.Workspace;
using Pladdra.Workspace.UxHandlers;
using Pladdra.Data;

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
                workspace.Actions.RegisterAction("default", (scene, workspace) => workspace.UseUxHandler(new AllowUserSelectWorkspaceActions()));
                workspace.Actions.RegisterAction("cancel-preview", (scene, workspace) => workspace.Actions.DispatchAction("default"));
                workspace.Actions.RegisterAction("preview", (scene, workspace) =>
                {
                    var s = workspace.GetSceneDescription();
                    workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(
                        s, new List<UserProposal>() { s }));
                });
            }
            else
            {
                workspace.Actions.RegisterAction("default", (scene, workspace) => workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(configuration.UserProposals.FirstOrDefault(), configuration.UserProposals)));
            }
            workspace.Actions.DispatchAction("default");
        }

        public ArDialogueRoomAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }
    }
}