using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;

namespace Repository
{
    public class ArDialogueRoomProjectRepository : DialogProjectRepository
    {
        public class WPModel
        {
            public class ACF
            {
                [JsonProperty("plane_dimensions_width")]
                public int PlaneDimensionsWidth { get; set; }
                [JsonProperty("plane_dimensions_height")]
                public int PlaneDimensionsHeight { get; set; }
                [JsonProperty("resources")]
                public List<Resource> Resources { get; set; }
            }
            public class Resource 
            {
                [JsonProperty("name")]
                public string Name { get; set; }
                [JsonProperty("model")]
                public string Model { get; set; }
                [JsonProperty("marker_model")]
                public string MarkerModel { get; set; }
            }
            [JsonProperty("acf")]
            public ACF Acf { get; set; }
        }
        
        
        
        private protected string CachePath { get; set; }
        private void Awake()
        {
            CachePath = Path.Join(Application.temporaryCachePath,"ar-dialogue-room");
        }
        
        public override async Task<DialogProject> Load()
        {
            var url =
                "https://modul-test.helsingborg.io/helsingborgsrummet/wp-json/wp/v2/ar-dialogue-room/16?acf_format=standard";
            var path = await new WebResourceManager(CachePath).GetResourcePath(url);

            var text = File.ReadAllText(path, Encoding.UTF8);
            var p = JsonConvert.DeserializeObject<WPModel>(text);
            
            Debug.Log(JsonConvert.SerializeObject(p, Formatting.Indented));
            
            return new DialogProject()
            {
                Id = url,
                Plane = new DialogPlane()
                {
                    Width = p.Acf?.PlaneDimensionsWidth ?? 5,
                    Height = p.Acf?.PlaneDimensionsHeight ?? 5
                },
                Resources = (p?.Acf?.Resources?.Select(r => new DialogResource()
                {
                    Id = r.Name,
                    Type = "model",
                    Url = r.Model
                })
                    .Where(r => !string.IsNullOrEmpty(r.Id))
                    .Where(r => !string.IsNullOrEmpty(r.Url))
                    ?? Enumerable.Empty<DialogResource>()).ToList()
            };
        }

        public override Task<DialogScene> SaveScene(DialogScene scene)
        {
            return Task.FromResult(scene);
        }

        public override Task<Dictionary<string, DialogScene>> LoadScenes()
        {
            return Task.FromResult(new Dictionary<string, DialogScene>());
        }
    }
}