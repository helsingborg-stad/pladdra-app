using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Abilities.ARRoomAbility.WP.Schema;
using Data;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;
using Utility;
using Pladdra.Data;

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
        
        public virtual async Task<Project> Load()
        {
            var model = await FetchModel();
            
            return new Project()
            {
                Id = Endpoint,
                // Plane = new DialogPlane()
                // {
                //     Width = model.Acf?.PlaneDimensionsWidth ?? 5,
                //     Height = model.Acf?.PlaneDimensionsHeight ?? 5
                // },
                Resources = (model?.Acf?.Resources?.Select(r => new PladdraResource()
                                 {
                                     Id = r.Name,
                                     Type = "model",
                                     ModelURL = r.Model,
                                     ModelIconURL = r.MarkerModel,
                                     Disable = r.Disable
                                 })
                                 .Where(r => !string.IsNullOrEmpty(r.Id))
                                 .Where(r => !string.IsNullOrEmpty(r.ModelURL))
                                 .Where(r => !string.IsNullOrEmpty(r.ModelIconURL))
                                 .Where(r => r.Disable != true)
                             ?? Enumerable.Empty<PladdraResource>()).ToList(),
                UserProposals = model?.Acf?.Scenes?
                    .Where(s => s != null)
                    .Where(s => s.IsFeatured)
                    .Select(TryParseScene)
                    .Where(s => s != null)
                    .ToList() ?? new List<UserProposal>(),
                Marker = model?.Acf.Marker != null ? new ARMarker()
                {
                    Image = model.Acf.Marker.Image,
                    Width = model.Acf.Marker.Width,
                    Height = model.Acf.Marker.Height,
                }: null
            };
        }
        
        public virtual async Task<UserProposal> SaveScene(UserProposal scene)
        {
            // SaveScene
            //  load existing room
            //  patch/add scene
            //  update existing room 
            var model = await new WebRestManager().GetJson<WpArDialogueRoomUpdate>(Endpoint);
            model.Acf ??= new WpArDialogueRoomUpdate.AdvancedCustomFields();
            model.Acf.Scenes ??= new List<WpScene>();

            var scenes = model.Acf.Scenes;

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

            var endpoint = UpdateQueryString(Endpoint, qs =>
            {
                // qs["_fields"] = "acf.scenes";
            });
            

            await new WebRestManager().PutJson(
                endpoint,
                model,
                headers: Headers
            );
            return scene;
        }

        public virtual async Task<Dictionary<string, UserProposal>> LoadScenes()
        {
            var model = await new WebRestManager().GetJson<WpArDialogueRoom>(Endpoint);

            return (model?.Acf?.Scenes ?? Enumerable.Empty<WpScene>())
                .Where(scene => scene != null)
                .Select(TryParseScene)
                .Where(scene => scene != null)
                .UniqueBy(scene => scene.Name)
                .ToDictionary(scene => scene.Name, scene => scene);
        }

        private async Task<WpArDialogueRoom> FetchModel()
        {
            return await new WebRestManager().GetJson<WpArDialogueRoom>(Endpoint);
        }

        private UserProposal TryParseScene(WpScene scene)
        {
            var parsed = TryParseJson<UserProposal>(scene.Json);
            if (parsed != null)
            {
                parsed.Name = scene.Name;
            }

            return parsed;
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

        private string UpdateQueryString(string url, Action<NameValueCollection> update)
        {
            var uri = new UriBuilder(url);
            var qs = HttpUtility.ParseQueryString(uri.Query);
            update(qs);
            uri.Query = qs.ToString();
            return uri.ToString();
        }
    }
}