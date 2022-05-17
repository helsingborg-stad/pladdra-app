using System.Threading.Tasks;
using Data.Dialogs;
using UnityEngine;
using Workspace.Snapshot;

namespace Repository
{
    public class RepositoryManager: MonoBehaviour, IDialogProjectRepository
    {
        [SerializeField]
        public DialogProjectRepository repository;

        public Task<DialogProject> Load()
        {
            return repository.Load();
        }

        public Task SaveScene(string name, WorkspaceSceneDescription scene)
        {
            return repository.SaveScene(name, scene);
        }
    }
}