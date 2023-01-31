using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pladdra.Lobby;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UntoldGarden.Utils;

namespace Pladdra
{
    /// <summary>
    /// Loads and handles abilities. 
    /// Requires a <see cref="AbilityList"/> with available abilities to be assigned.
    /// Create a script that implements <see cref="IDeepLinkHandler"/> to handle the deeplink after an ability has been loaded.
    /// </summary>
    public class AbilityManager : MonoBehaviour
    {
        #region Public
        public AbilityList abilityList;
        [SerializeField] LobbyManager lobbyManager;
        [SerializeField] UIManager uiManager;
        #endregion Public

        #region MonoBehaviour
        void Awake()
        {
            if (lobbyManager != null && abilityList != null)
                lobbyManager.ShowAbilityList(abilityList.abilities, OpenAbility);

            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            Application.deepLinkActivated += url => OpenDeepLink(url);
        }

        void OnDisable()
        {
            Application.deepLinkActivated -= url => OpenDeepLink(url);
        }

        #endregion MonoBehaviour

        #region Ability Handling

        /// <summary>
        /// Opens an ability by loading its scene.
        /// Triggered from UI input in <see cref="LobbyManager.ShowAbilityList(List{Ability}, Action{Ability})"/>
        /// </summary>
        /// <param name="ability">The ability to open.</param>
        public void OpenAbility(Ability ability)
        {
            if (IsValidAbility(ability))
            {
                SceneManager.LoadScene(ability.scene);
            }
        }

        /// <summary>
        /// Opens an ability by loading its scene and passing the deeplink to the first <see cref="IDeepLinkHandler"/> found.
        /// Triggered from deeplink.
        /// </summary>
        /// <param name="ability">The ability to open.</param>
        /// <param name="deeplink">The deeplink that opened the ability.</param>
        public void OpenAbility(Ability ability, string deeplink)
        {
            if (IsValidAbility(ability, deeplink))
            {
                SceneManager.LoadScene(ability.scene);
                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    var deeplinkHandler = FindObjectsOfType<MonoBehaviour>().OfType<IDeepLinkHandler>().FirstOrDefault();
                    if (deeplinkHandler != null)
                    {
                        deeplinkHandler.OpenDeepLink(deeplink);
                    }
                };
            }
        }

        /// <summary>
        /// Checks if the ability is valid and has a scene. 
        /// Shows an error if not.
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        bool IsValidAbility(Ability ability, string url = "")
        {
            string error = "";
            if (ability == null)
                error = $"Ability not found. {url}";
            else if (ability.scene.IsNullOrEmptyOrFalse())
                error = $"Could not find scene for ability {ability.name}";
            else
                return true;

            if (uiManager != null)
                uiManager.ShowError(error);
            else
                Debug.LogError(error);
            return false;
        }
        #endregion Ability Handling

        #region DeepLink Handling

        /// <summary>
        /// Called from <see cref="Application.deepLinkActivated"/>.
        /// </summary>
        /// <param name="url">The DeepLink.</param>
        public void OpenDeepLink(string url)
        {
            OpenAbility(abilityList.abilities.Find(x => x.deepLinkIdentifiers.Contains(new Uri(url).Host)), url);
        }
        #endregion DeepLink Handling
    }
}