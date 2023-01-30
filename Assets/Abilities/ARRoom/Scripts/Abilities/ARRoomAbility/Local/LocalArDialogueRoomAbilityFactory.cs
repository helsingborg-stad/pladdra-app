namespace Abilities.ARRoomAbility.Local
{
    public class LocalArDialogueRoomAbilityFactory : IAbilityFactory
    {
        private string TempPath { get; }
        public LocalArDialogueRoomAbilityFactory(string tempPath)
        {
            TempPath = tempPath;
        }
        public IAbility TryCreateAbility(string abilityName, string configJson)
        {
            return abilityName == "ar-dialogue-room-local"
                ? new ArDialogueRoomAbility(new LocalRepository(TempPath)) : null;
        }
    }
}