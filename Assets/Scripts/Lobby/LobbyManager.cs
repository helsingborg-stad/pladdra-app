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
        #region Private

        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        bool afterStart;
        #endregion Private

        public void ShowAbilityList(List<Ability> abilities, Action<Ability> openAbility)
        {
            if (uiManager == null)
            {
                Debug.LogError("UIManager is null");
                return;
            }

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
                   openAbility(ability);
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
    }
}