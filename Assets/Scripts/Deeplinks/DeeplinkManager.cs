using System;
using System.IO;
using System.Linq;
using Abilities;
using Abilities.ARRoomAbility.Local;
using Abilities.ARRoomAbility.WP;
using ExampleScreens;
using Screens;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pladdra;

namespace Deeplinks //TODO Change to Pladdra
{
    public class DeeplinkManager : MonoBehaviour
    {
        #region Public members
        [SerializeField] string testDeeplink;
        #endregion
        
        #region Private members
        protected bool afterStart;
        protected ProjectManager projectManager { get{ return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); }}
        #endregion
        private void Awake()
        {
            Application.deepLinkActivated += url => TryNavigateDeeplink(url);

            // var didNavigate = TryNavigateDeeplink(Application.absoluteURL) || TryNavigateDeeplink(deeplink == "" ? TryReadDevelopmentDeeplink() : deeplink);
            TryNavigateDeeplink(testDeeplink == "" ? TryReadDevelopmentDeeplink() : testDeeplink);
        }

        private void Start()
        {
            afterStart = true;
        }

        private void TryNavigateDeeplink(string url)
        {
            if(projectManager == null)
            {
                Debug.Log("ProjectManager is null");
                return;
            }
            if (afterStart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            // Look over this and simplify, no need for 5 different classes
            // TODO Add JSON schema in the beginning of the url instead of the admin stuff
            var ability = AbilityUri.TryCreateAbility(url,
                new WpArDialogueRoomForAdminAbilityFactory(),
                new WpArDialogueRoomForVisitorAbilityFactory(),
                new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath)
            );
            if (ability != null)
            {
                projectManager.LoadProject(ability);
            }
        }

        // private bool TryNavigateDeeplink(string url)
        // {
        //     if (afterStart)
        //     {
        //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //     }
        //     var ability = AbilityUri.TryCreateAbility(url,
        //         new WpArDialogueRoomForAdminAbilityFactory(),
        //         new WpArDialogueRoomForVisitorAbilityFactory(),
        //         new LocalArDialogueRoomAbilityFactory(Application.temporaryCachePath)
        //     );
        //     if (ability != null)
        //     {
        //         FindObjectOfType<ScreenManager>().SetActiveScreen<WelcomeScreen>(
        //             beforeActivate: screen => screen.Configure(ability));
        //         return true;
        //     }

        //     return false;
        // }

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