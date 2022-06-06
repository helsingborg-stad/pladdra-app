using Workspace;

namespace Abilities.ARRoomAbility
{
    public class Layers
    {
        public static readonly IWorkspaceLayer Marker = new WorkspaceLayer()
        {
            Name = "marker",
            Label = "Markörer"
        };
        public static readonly  IWorkspaceLayer Model = new WorkspaceLayer()
        {
            Name = "model",
            Label = "Modeller"
        };
    }
}