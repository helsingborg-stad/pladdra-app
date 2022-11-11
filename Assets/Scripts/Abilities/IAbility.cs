using Abilities.ARRoomAbility;
using Pladdra.Workspace;

namespace Abilities
{
    public interface IAbility
    {
        IDialogProjectRepository Repository { get; }
        void ConfigureWorkspace(WorkspaceConfiguration configuration, IWorkspace workspace);
    }
}