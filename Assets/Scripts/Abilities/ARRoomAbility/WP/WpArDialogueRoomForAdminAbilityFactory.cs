namespace Abilities.ARRoomAbility.WP
{
    public class WpArDialogueRoomForAdminAbilityFactory : WpArDialogueRoomAbilityFactory
    {
        protected override bool GetEditMode() => true;

        protected override string GetAbilityName() => "ar-dialogue-room-admin";
    }
}