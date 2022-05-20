using Repository;

namespace Abilities
{
    public interface IAbility
    {
        IDialogProjectRepository Repository { get; }
    }
}