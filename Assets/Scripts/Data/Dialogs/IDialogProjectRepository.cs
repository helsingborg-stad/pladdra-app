using System.Threading.Tasks;
using Workspace.Snapshot;

namespace Data.Dialogs
{
    public interface IDialogProjectRepository {
        Task<DialogProject> Load ();
        Task SaveScene(string name, WorkspaceSceneDescription scene);
    }
}
