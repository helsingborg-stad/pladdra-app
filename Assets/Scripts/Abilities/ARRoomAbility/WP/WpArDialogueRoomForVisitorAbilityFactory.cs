namespace Abilities.ARRoomAbility.WP
{
    public class WpArDialogueRoomForVisitorAbilityFactory : WpArDialogueRoomAbilityFactory
    {
        protected override bool GetEditMode() => false;

        protected override string GetAbilityName() => "ar-dialogue-room";
    }
}