using System;
using System.Linq;
using System.Text;

namespace Abilities
{
    public class CreateAbilityFromUri
    {
        public IAbility TryCreateAbility(Uri uri, params IAbilityFactory[] factories)
        {
            // the uri should be like pladdra://<abilityName>/<abilityConfig>
            if (uri.Scheme != "pladdra")
            {
                return null;
            }

            var abilityName = uri.Authority;
            var abilityConfig = TryDecodePathToJson(uri.AbsolutePath);

            return factories
                .Select(f => f.TryCreateAbility(abilityName, abilityConfig))
                .FirstOrDefault(f => f != null);
        }

        private string TryDecodePathToJson(string path)
        {
            try
            {
                return path
                    .Split('/')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(Convert.FromBase64String)
                    .Select(Encoding.UTF8.GetString)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}