using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;

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
                Plane = new DialogPlane(){Width = 4, Height = 4},
                Resources = SampleModels.Select(url => new DialogResource{Url = url, Type = "model"}).ToList()
        });

        public override Task<DialogScene> SaveScene(DialogScene scene)
        {
            var path = Path.Combine(TempPath, "scenes", $"{scene.Name}.scene.json");
            Debug.Log($"Saving scene {scene.Name} to {path}");

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            File.WriteAllText(path, JsonConvert.SerializeObject(scene, Formatting.Indented), Encoding.UTF8);
            return Task.FromResult(scene);
        }

        public override Task<Dictionary<string, DialogScene>> LoadScenes()
        {
            var path = Path.Combine(TempPath, "scenes");
            try
            {
                var result = Directory.EnumerateFiles(path, "*.scene.json")
                    .Select(path => new { path, name = TrimSuffix(Path.GetFileName(path), ".scene.json") })
                    .Select(o => new
                    {
                        name = o.name,
                        scene = TryLoadScene(o.path)
                    })
                    .Where(o => o.scene != null)
                    .ToDictionary(o => o.name, o => PatchScene(o.name, o.scene));
                return Task.FromResult(result);
            }
            catch (DirectoryNotFoundException)
            {
                return Task.FromResult(new Dictionary<string, DialogScene>());
            }
        }

        private DialogScene TryLoadScene(string path)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<DialogScene>(text);
        }

        private DialogScene PatchScene(string name, DialogScene scene)
        {
            // We patch in nane in the scene since it wasn't previously persisted
            scene.Name = string.IsNullOrEmpty(scene.Name) ? name : scene.Name;
            return scene;
        }

        private string TrimSuffix(string value, string suffix) => value.EndsWith(suffix) ? value.Remove(value.Length - suffix.Length) : value;
    }
}
