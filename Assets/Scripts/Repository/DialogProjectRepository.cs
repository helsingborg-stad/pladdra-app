using System.Threading.Tasks;
using Data.Dialogs;
using UnityEngine;
using Workspace.Snapshot;

namespace Repository
{
    public abstract class DialogProjectRepository : MonoBehaviour, IDialogProjectRepository
    {
        public abstract Task<DialogProject> Load();
        public abstract Task SaveScene(string name, WorkspaceSceneDescription scene);
    }
}