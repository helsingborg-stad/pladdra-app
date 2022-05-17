using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;
using Workspace.Snapshot;

namespace Repository
{
    public class SampleDialogProjectRepository: DialogProjectRepository {
        
        static string[] SampleModels = new string[] {
            // "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/03/telefonare.glb",
            //"https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/03/lego_test.glb",
            //"https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/02/svensgardsskolan_4.glb"
            "https://helsingborgs-rummet.s3.eu-north-1.amazonaws.com/ballong.glb",
            "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/05/7-museum-maria-park.glb",
            "https://modul-test.helsingborg.io/wp-content/uploads/sites/30/2022/05/20-lekplats-1.glb",
        };

        protected string TempPath { get; private set;  }

        public void Awake()
        {
            TempPath = Application.temporaryCachePath;
        }
        
        public override Task<DialogProject> Load() => Task.FromResult(new DialogProject() {
                Id = "dialog-1",
                Resources = SampleModels.Select(url => new DialogResource{Url = url, Type = "model"}).ToList()
        });

        public override Task SaveScene(string name, WorkspaceSceneDescription scene)
        {
            var path = Path.Combine(TempPath, "scenes", $"{name}.scene.json");
            Debug.Log($"Saving scene {name} to {path}");

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            File.WriteAllText(path, JsonConvert.SerializeObject(scene, Formatting.Indented), Encoding.UTF8);
            return Task.FromResult(0);
        }
    }
}
