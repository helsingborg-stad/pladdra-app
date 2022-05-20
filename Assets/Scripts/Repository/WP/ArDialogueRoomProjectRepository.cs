using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Data;
using Data.Dialogs;
using Newtonsoft.Json;
using Repository.WP.Schema;
using UnityEngine;
using Utility;

namespace Repository.WP
{
    public class ArDialogueRoomProjectRepository : DialogProjectRepository
    {
        private string Auth = "aGVsc2luZ2JvcmdzcnVtbWV0Ok96enMgSDBZVyBXOWtqIFMxd1cgcU12VCBLN2hZ";
        private string Endpoint { get; } =
            "https://modul-test.helsingborg.io/helsingborgsrummet/wp-json/wp/v2/ar-dialogue-room/16?acf_format=standard";


        private string CachePath { get; set; }
        private void Awake()
        {
            CachePath = Path.Join(Application.temporaryCachePath,"ar-dialogue-room");
        }
        
        public override async Task<DialogProject> Load()
        {
            var model = await FetchModel();
            Debug.Log(JsonConvert.SerializeObject(model, Formatting.Indented));
            
            return new DialogProject()
            {
                Id = Endpoint,
                Plane = new DialogPlane()
                {
                    Width = model.Acf?.PlaneDimensionsWidth ?? 5,
                    Height = model.Acf?.PlaneDimensionsHeight ?? 5
                },
                Resources = (model?.Acf?.Resources?.Select(r => new DialogResource()
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

        public override async Task<DialogScene> SaveScene(DialogScene scene)
        {
            // SaveScene
            //  load existing room
            //  patch/add scene
            //  update existing room 
            var model = await new WebRestManager().GetJson<WpArDialogueRoom>(Endpoint);

            var scenes = model?.Acf?.Scenes ?? new List<WpScene>();

            var existingScene = scenes.Find(existing => existing.Name == scene.Name);
            if (existingScene != null)
            {
                existingScene.Json = JsonConvert.SerializeObject(scene);
            }
            else
            {
                scenes.Add(new WpScene()
                {
                    Name = scene.Name,
                    Json = JsonConvert.SerializeObject(scene)
                });
            }

            await new WebRestManager().PutJson(Endpoint,
                new WpUpdateScenes() { Acf = new WpUpdateScenes.AdvancedCustomFields() { Scenes = scenes } },
                AuthorizeRequest
                );
            return scene;
        }

        public override async Task<Dictionary<string, DialogScene>> LoadScenes()
        {
            var model = await new WebRestManager().GetJson<WpArDialogueRoom>(Endpoint);

            return (model?.Acf?.Scenes ?? Enumerable.Empty<WpScene>())
                .Where(scene => scene != null)
                .Select(scene => new
                {
                    source = scene,
                    parsed = TryParseJson<DialogScene>(scene.Json)
                })
                .Where(o => o.parsed != null)
                .UniqueBy(o => o.source.Name)
                .ToDictionary(o => o.source.Name, o => o.parsed);
        }

        private async Task<WpArDialogueRoom> FetchModel()
        {
            return await new WebRestManager().GetJson<WpArDialogueRoom>(Endpoint);
        }
        private T TryParseJson<T>(string json) where T: class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AuthorizeRequest(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Auth);
        }
    }
}