using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dialogs;
using UnityEngine;

namespace Repository
{
    public abstract class DialogProjectRepository : MonoBehaviour, IDialogProjectRepository
    {
        public abstract Task<DialogProject> Load();
        public abstract Task<DialogScene> SaveScene(string name, DialogScene scene);
        public abstract Task<Dictionary<string, DialogScene>> LoadScenes();
    }
}