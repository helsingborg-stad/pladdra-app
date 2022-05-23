using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Abilities.ARRoomAbility.WP
{
    public abstract class WpArDialogueRoomAbilityFactory : IAbilityFactory
    {
        public class Config
        {
            [JsonProperty("endpoint")] public string Endpoint { get; set; }
            [JsonProperty("headers")] public Dictionary<string, string> Headers { get; set; }
        }

        public IAbility TryCreateAbility(string abilityName, string configJson)
        {
            return abilityName == GetAbilityName() ? TryCreateAbilityFromConfig(configJson) : null;
        }

        private IAbility TryCreateAbilityFromConfig(string configJson)
        {
            var config = TryParse<Config>(configJson);
            
            return string.IsNullOrEmpty(config?.Endpoint) ? null : new ArDialogueRoomAbility(
                new WpRepository()
                {
                    Endpoint = config.Endpoint,
                    Headers = config.Headers ?? new Dictionary<string, string>()
                })
            {
                IsEditMode = GetEditMode()
            };
        }

        protected abstract bool GetEditMode();
        protected abstract string GetAbilityName();
 
        T TryParse<T>(string json) where T : class
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
    }
}

    
