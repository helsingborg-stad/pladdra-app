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
    public class DeeplinkManager : MonoBehaviour
    {
        private void Awake()
        {
            Application.deepLinkActivated += url => TryNavigateDeeplink(url);

            var deeplinked =
                new[]
                    {
                        Application.absoluteURL, TryReadDevelopmentDeeplink()
                    }
                    .Select(TryNavigateDeeplink)
                    .FirstOrDefault();
        }

        private bool TryNavigateDeeplink(string url)
        {
            Debug.Log(url);
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
                return true;
            }

            return false;
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