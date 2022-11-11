using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Abilities
{
    public static class AbilityUri
    {
        public static IAbility TryCreateAbility(string uri, params IAbilityFactory[] factories)
        {
            var parsed = default(Uri);
            return Uri.TryCreate(uri, UriKind.Absolute, out parsed) ? TryCreateAbility(parsed, factories) : null;
        }
        public static IAbility TryCreateAbility(Uri uri, params IAbilityFactory[] factories)
        {
            // the uri should be like pladdra://<abilityName>/<abilityConfig>
            if (uri?.Scheme != "pladdra")
            {
                return null;
            }

            var abilityName = uri.Authority;
            var abilityConfig = TryDecodePathToJson(uri.AbsolutePath.TrimStart('/'));
            Debug.Log($"AbilityUri: {abilityName} config {abilityConfig}");

            return factories
                .Select(f => f.TryCreateAbility(abilityName, abilityConfig))
                .FirstOrDefault(f => f != null);
        }

        public static Uri CreateAbilityUri(string ability, object abilityConfiguration)
        {
            return new UriBuilder()
            {
                Scheme = "pladdra",
                Host = ability,
                Path = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(abilityConfiguration)))
            }.Uri;
        }

        private static string TryDecodePathToJson(string path)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(path));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}