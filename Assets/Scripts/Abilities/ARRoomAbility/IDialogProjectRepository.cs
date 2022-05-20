using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dialogs;

namespace Abilities.ARRoomAbility
{
    public interface IDialogProjectRepository {
        Task<DialogProject> Load ();
        Task<DialogScene> SaveScene(DialogScene scene);
        Task<Dictionary<string, DialogScene>> LoadScenes();
    }
}
