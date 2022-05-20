using System;
using System.Linq;

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
            var abilityConfig = uri.AbsolutePath;

            return factories
                .Select(f => f.TryCreateAbility(abilityName, abilityConfig))
                .FirstOrDefault(f => f != null);
        }
    }
    public interface IAbilityFactory
    {
        IAbility TryCreateAbility(string abilityName, string abilityConfig);
    }
    public interface IAbility {}
    
}