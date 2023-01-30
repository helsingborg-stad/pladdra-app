using Abilities;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class WelcomeScreen : Screen
    {
         private void Start()
         {
             if (Ability != null)
             {
                 ScreenManager.SetActiveScreen<LoadProjectsScreen>(
                     beforeActivate: screen => screen.Ability = Ability);
             }
         }

         public void Configure(IAbility ability)
         {
             Ability = ability;
         }

         private IAbility Ability { get; set; }
    }
}