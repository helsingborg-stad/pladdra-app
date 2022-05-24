using System;
using System.IO;
using System.Linq;
using Abilities;
using Abilities.ARRoomAbility.Local;
using Abilities.ARRoomAbility.WP;
using ExampleScreens;
using Screens;
using UnityEngine;

namespace Deeplinks
{
    public class DeeplinkManager: MonoBehaviour
    {
        private void Start()
        {
            Application.deepLinkActivated += url => TryNavigateDeeplink(url);
            TryNavigateDeeplink(TryReadDevelopmentDeeplink());
        }

        private void TryNavigateDeeplink(string url)
        {
            Debug.Log(url);
            var ability = AbilityUri.TryCreateAbility(url,
                new WpArDialogueRoomForAdminAbilityFactory(),
                new WpArDialogueRoomForVisitorAbilityFactory(),
                new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath)
            );
            if (ability != null)
            {
                FindObjectOfType<ScreenManager>().SetActiveScreen<WelcomeScreen>(
                    beforeActivate: screen => screen.Configure(ability));
            }
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
    }
}