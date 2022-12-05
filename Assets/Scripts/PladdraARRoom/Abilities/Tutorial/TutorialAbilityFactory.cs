using System;
namespace Abilities.Tutorial
{
    /// <summary>
    /// Ability factory for the Tutorial Ability.
    /// The overall purpose of Tutorial is to provide a playground
    /// for onboarding and learning Pladdra architectural concepts
    /// </summary>
    /// <remarks>
    /// Handle deeplinks formatted as "pladdra://tutorial"
    /// </remarks>
	public class TutorialAbilityFactory: IAbilityFactory
	{
		public TutorialAbilityFactory()
		{
		}

        public IAbility TryCreateAbility(string abilityName, string configJson)
        {
            if (!string.Equals(abilityName, "tutorial"))
            {
                return null;
            }
            return new TutorialAbility(new TutorialRepository());
        }
    }
}

