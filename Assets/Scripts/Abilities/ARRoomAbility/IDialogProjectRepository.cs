using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dialogs;
using Pladdra.Data;

namespace Abilities.ARRoomAbility
{
    public interface IDialogProjectRepository {
        Task<Project> Load ();
        Task<UserProposal> SaveScene(UserProposal scene);
        Task<Dictionary<string, UserProposal>> LoadScenes();
    }
}
