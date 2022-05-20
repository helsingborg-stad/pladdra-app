using System;
using System.Collections.Generic;
using Abilities.ARRoomAbility.WP;
using Newtonsoft.Json;

namespace Abilities.ARRoomAbility
{
    public class WpArDialogueRoomAbilityFactory : IAbilityFactory
    {
        public class Config
        {
            [JsonProperty("endpoint")] public string Endpoint { get; set; }
            [JsonProperty("headers")] public Dictionary<string, string> Headers { get; set; }
        }

        public IAbility TryCreateAbility(string abilityName, string abilityConfig)
        {
            return abilityName == "ar-dialogue-room" ? TryCreateAbilityFromConfig(abilityConfig) : null;
        }

        private IAbility TryCreateAbilityFromConfig(string configJson)
        {
            var config = TryParse<Config>(configJson);
            return string.IsNullOrEmpty(config?.Endpoint) ? null : new ArDialogueRoomAbility(
                new WpRepository()
                {
                    Endpoint = config.Endpoint,
                    Headers = config.Headers ?? new Dictionary<string, string>()
                });
        }

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

    
