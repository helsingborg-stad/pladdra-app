using System.Threading.Tasks;

namespace Data.Dialogs
{
    public interface IDialogProjectRepository {
        Task<DialogProject> Load ();
    }
}
