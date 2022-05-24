using System;
using System.Linq;
using Abilities;
using Abilities.ARRoomAbility.Local;
using Abilities.ARRoomAbility.WP;
using Screens;
using UnityEngine;
using UnityEngine.Windows;
using File = System.IO.File;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class WelcomeScreen : Screen
    {
         private void Start()
         {
             if (Ability != null)
             {
                 GetComponentInParent<ScreenManager>().SetActiveScreen<LoadProjectsScreen>(
                     beforeActivate: screen => screen.Ability = Ability);
             }
             /*
             var uri = TryReadDevelopmentDeeplink();
             var ability = AbilityUri.TryCreateAbility(uri,
                 new WpArDialogueRoomForAdminAbilityFactory(),
                 new WpArDialogueRoomForVisitorAbilityFactory(),
                 new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath)
             );
             
             GetComponentInParent<ScreenManager>().SetActiveScreen<LoadProjectsScreen>(
                 beforeActivate: screen => screen.Ability = ability);
            */
         }

         private string TryReadDevelopmentDeeplink()
         {
             try
             {
                 return File
                     .ReadAllLines("deeplink.dev")
                     .Where(line => !string.IsNullOrEmpty(line))
                     .FirstOrDefault(line => line.StartsWith("pladdra://"));
             }
             catch (Exception)
             {
                 return null;
             }
         }

         public void Configure(IAbility ability)
         {
             Ability = ability;
         }

         private IAbility Ability { get; set; }
    }
}