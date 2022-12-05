using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuManager : MonoBehaviour
    {
        public VisualTreeAsset menuButtonTemplate;
        UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        AppManager appManager { get { return transform.parent.gameObject.GetComponentInChildren<AppManager>(); } }
        Button menuButton;
        VisualElement menu;
        bool menuOpen;

        List<MenuItem> menuItems = new List<MenuItem>();

        void Start()
        {
            menu = uiDocument.rootVisualElement.Q<VisualElement>("menu");
            menu.visible = false;

            // Add default menu items
            // TODO Move this to the inspector?
            menuItems.Add(new MenuItem()
            {
                id = "project-list",
                name = "Project List",
                action = () =>
                {
                    appManager.ShowTestProjectList();
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "about",
                name = "About",
                action = () =>
                {
                    uiManager.ShowUI("about-pladdra", root =>
                            {
                                root.Q<Button>("close").clicked += () => { uiManager.ShowPreviousUI(); };
                            });
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "contact",
                name = "Contact",
                action = () =>
                {
                    Application.OpenURL("mailto:jakob@untold.garden");
                }
            });

            uiDocument.rootVisualElement.Q<Button>("menu-button").clicked += () =>
            {
                ToggleMenu();
            };

            PopulateMenu();
        }

        /// <summary>
        /// Adds all menuItems to the menu
        /// </summary>
        void PopulateMenu()
        {
            ListView menulist = uiDocument.rootVisualElement.Q<ListView>("menu-items");

            menulist.makeItem = () =>
            {
                var button = menuButtonTemplate.Instantiate();
                return button;
            };
            menulist.bindItem = (element, i) =>
            {
                Button button = element.Q<Button>();
                button.text = menuItems[i].name;
                button.clicked += () =>
                {
                    menuItems[i].action();
                    ToggleMenu();
                };
            };
            menulist.fixedItemHeight = 50;
            menulist.itemsSource = menuItems.ToArray();
        }

        /// <summary>
        /// Toggles menu visible and hidden
        /// </summary>
        void ToggleMenu()
        {
            menuOpen = !menuOpen;
            menu.visible = menuOpen;
        }

        /// <summary>
        /// Adds a new menu item with an action to the menu
        /// </summary>
        /// <param name="item">The menu item to add</param>
        public void AddMenuItem(MenuItem item)
        {
            menuItems.Add(item);
            PopulateMenu();
        }

        public void ToggleMenuButtonVisibility(bool visible)
        {
            uiDocument.rootVisualElement.Q<Button>("menu-button").visible = visible;
        }

    }

    public class MenuItem
    {
        public string id;
        public string name;
        public Texture2D Icon;
        public Action action;
    }
}