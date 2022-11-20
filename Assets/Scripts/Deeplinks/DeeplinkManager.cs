using System;
using System.IO;
using System.Linq;
using Abilities;
using Abilities.ARRoomAbility.Local;
using Abilities.ARRoomAbility.WP;
using Abilities.Tutorial;
using ExampleScreens;
using Screens;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deeplinks
{
    public class DeeplinkManager : MonoBehaviour
    {
        protected bool afterStart;
        private void Awake()
        {
            Application.deepLinkActivated += url => TryNavigateDeeplink(url);

            var didNavigate = TryNavigateDeeplink(Application.absoluteURL) || TryNavigateDeeplink(TryReadDevelopmentDeeplink());
        }
        
        private void Start()
        {
            afterStart = true;
        }

        private bool TryNavigateDeeplink(string url)
        {
            if (afterStart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            var ability = AbilityUri.TryCreateAbility(url,
                new WpArDialogueRoomForAdminAbilityFactory(),
                new WpArDialogueRoomForVisitorAbilityFactory(),
                new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath),
                new TutorialAbilityFactory()
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