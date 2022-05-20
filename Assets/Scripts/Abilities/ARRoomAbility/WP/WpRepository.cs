using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abilities.ARRoomAbility.WP.Schema;
using Data;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace Abilities.ARRoomAbility.WP
{
    public class WpRepository : IDialogProjectRepository
    {

        private string CachePath { get; set; }
    
        public string Endpoint { get; set; }
        public Dictionary<string,string> Headers { get; set; }

        private void Awake()
        {
            CachePath = Path.Join(Application.temporaryCachePath,"ar-dialogue-room");
        }
        
        public virtual async Task<DialogProject> Load()
        {
            var model = await FetchModel();
            
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
                             ?? Enumerable.Empty<DialogResource>()).ToList(),
                FeaturedScenes = model?.Acf?.Scenes?
                    .Where(s => s != null)
                    .Where(s => s.IsFeatured)
                    .Select(s => TryParseJson<DialogScene>(s.Json))
                    .Where(s => s != null)
                    .ToList() ?? new List<DialogScene>()
            };
        }

        public virtual async Task<DialogScene> SaveScene(DialogScene scene)
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
                SetHeaders
            );
            return scene;
        }

        public virtual async Task<Dictionary<string, DialogScene>> LoadScenes()
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

        private void SetHeaders(HttpRequestMessage request)
        {
            foreach (var kv in Headers)
            {
                request.Headers.Add(kv.Key, kv.Value);
            }
        }
    }
}