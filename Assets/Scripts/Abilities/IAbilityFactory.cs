namespace Abilities
{
    public interface IAbilityFactory
    {
        IAbility TryCreateAbility(string abilityName, string configJson);
    }
}