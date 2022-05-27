using System;
using UnityEngine;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class ErrorScreen: Screen
    {
        public void Configure(Exception exception)
        {
            Debug.Log(exception);
        }

        protected override void AfterActivateScreen()
        {
            HudManager.UseHud("app-has-error-hud", root => {});
        }
    }
}