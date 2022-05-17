using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dialogs;
using UnityEngine;

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

        public Task<DialogScene> SaveScene(string name, DialogScene scene)
        {
            return repository.SaveScene(name, scene);
        }

        public Task<Dictionary<string, DialogScene>> LoadScenes()
        {
            return repository.LoadScenes();
        }
    }
}