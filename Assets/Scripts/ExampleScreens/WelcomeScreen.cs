using Screens;

namespace ExampleScreens
{
    public class WelcomeScreen : Screen
    {
         private void Start() {
            GetComponentInParent<ScreenManager>().SetActiveScreen<LoadProjectsScreen>();
         }
   }
}