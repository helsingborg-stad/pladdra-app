using Repository;

namespace Abilities.ARRoomAbility
{
    public class ArDialogueRoomAbility : IAbility
    {
        public IDialogProjectRepository Repository { get; }

        public ArDialogueRoomAbility(IDialogProjectRepository repository)
        {
            Repository = repository;
        }
    }
}