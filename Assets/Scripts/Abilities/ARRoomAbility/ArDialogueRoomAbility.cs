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
                workspace.UseUxHandler(new AllowUserSelectWorkspaceActions());
            }
            else
            {
                workspace.UseUxHandler(new AllowUserToEnjoyTheRoom(configuration.FeaturedScenes.FirstOrDefault(), configuration.FeaturedScenes));
            }
        }

        public ArDialogueRoomAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }
    }
}