using System.Linq;
using Abilities.ARRoomAbility.UxHandlers;
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
                // Workspace.UseScene(wc.Scene);
                workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
            }
            else
            {
                workspace.UseScene(configuration.FeaturedScenes.FirstOrDefault());
                workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(configuration.FeaturedScenes));
            }
        }

        public ArDialogueRoomAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }
    }
}