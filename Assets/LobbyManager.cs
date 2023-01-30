using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pladdra.UI;
using System;
using UnityEngine.UIElements;

namespace Pladdra.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        #region Public
        public AbilityList abilityList;
        #endregion Public

        #region Private

        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        bool afterStart;
        #endregion Private

        void Awake()
        {
            Application.deepLinkActivated += url => OpenDeeplink(url);
        }
        void Start()
        {
            afterStart = true;
            ShowAbilityList();
        }

        public void ShowAbilityList()
        {
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null");
                return;
            }
            List<Ability> abilities = abilityList.abilities;

            if (abilities == null || abilities.Count == 0)
            {
                Debug.Log("Ability List is null or empty");
                // TODO
                return;
            }

            Action[] actions = new Action[abilities.Count];
            for (int i = 0; i < abilities.Count; i++)
            {
                int index = i;
                Ability ability = abilities[i];
                actions[i] = () =>
                {
                    SceneManager.LoadScene(ability.scene);
                };
            }

            uiManager.DisplayUI("ability-list", root =>
            {
                ListView listView = root.Q<ListView>("abilities");
                listView.makeItem = () =>
                                {
                                    var button = uiManager.uiAssets.Find(x => x.name == "project-button-template").visualTreeAsset.Instantiate();
                                    return button;
                                };
                listView.bindItem = (element, i) =>
                                {


                                    element.Q<Button>("project-button").clicked += () => { actions[i](); };

                                    element.Q<Label>("name").text = abilities[i].name;
                                    element.Q<Label>("description").text = abilities[i].description;
                                };
                listView.fixedItemHeight = 100;
                listView.itemsSource = abilities;
            });
        }

        /// <summary>
        /// Opens a project from a deeplink
        /// </summary>
        /// <param name="url"></param>
        void OpenDeeplink(string url)
        {
            // TODO Ask user to save if they have unsaved changes from previus project
            if (afterStart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // TODO Open deeplink
        }
    }
}