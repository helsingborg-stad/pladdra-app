using System.Linq;
using System.Threading.Tasks;

namespace Data.Dialogs
{
    public class SampleDialogProjectRepository: IDialogProjectRepository {
        
        static string[] SampleModels = new string[] {
            // "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/03/telefonare.glb",
            //"https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/03/lego_test.glb",
            //"https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/02/svensgardsskolan_4.glb"
            "https://helsingborgs-rummet.s3.eu-north-1.amazonaws.com/ballong.glb",
            "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/05/7-museum-maria-park.glb",
            "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/05/20-lekplats-1.glb",
        };

        public SampleDialogProjectRepository(string tempPath)
        {
            TempPath = tempPath;
        }

        private string TempPath { get; }

        public Task<DialogProject> Load() => Task.FromResult(new DialogProject() {
                Id = "dialog-1",
                Resources = SampleModels.Select(url => new DialogResource{Url = url, Type = "model"}).ToList()
        });
    }
}
