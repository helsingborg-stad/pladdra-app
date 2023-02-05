using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.UX;
using UnityEngine;
using UnityEngine.UIElements;
using Pladdra.ARSandbox.Dialogues;

using Pladdra.ARSandbox;
using Pladdra.UX;

namespace Pladdra.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuManager : MonoBehaviour
    {
        #region Public
        [SerializeField] VisualTreeAsset menuButtonTemplate;
        #endregion Public

        #region Scene References
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        protected AppManager appManager { get { return transform.parent.gameObject.GetComponentInChildren<AppManager>(); } }
        protected IUXManager uxManager { get { return transform.parent.gameObject.GetComponentInChildren<IUXManager>(); } }
        #endregion Scene References


        #region Private
        VisualElement menu;
        bool menuOpen;
        private List<VisualElement> NonMenuBlockingUIElements = new List<VisualElement>();
        protected List<MenuItem> menuItems = new List<MenuItem>();
        #endregion Private

        protected virtual void Start()
        {
            menu = uiDocument.rootVisualElement.Q<VisualElement>("menu");
            menu.visible = false;

            // Add default menu items
            menuItems.Add(new MenuItem()
            {
                id = "about",
                name = "Om Pladdra",
                action = () =>
                {
                    uiManager.DisplayUI("about-pladdra", root =>
                            {
                                root.Q<Button>("close").clicked += () =>
                                {
                                    uxManager.UseLastUX();
                                };
                                ToggleMenu(false);

                            });
                }
            });
            menuItems.Add(new MenuItem()
            {
                id = "contact",
                name = "Kontakta support",
                action = () =>
                {
                    Application.OpenURL("mailto:support@untold.garden");
                    ToggleMenu(true);
                }
            });
            
            uiDocument.rootVisualElement.Q<Button>("menu-button").clicked += () =>
            {
                ToggleMenu(true);
            };

            PopulateMenu();
        }

        /// <summary>
        /// Adds all menuItems to the menu
        /// </summary>
        protected void PopulateMenu()
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
                };
            };
            menulist.fixedItemHeight = 50;
            menulist.itemsSource = menuItems.ToArray();
            menulist.Rebuild();

        }

        /// <summary>
        /// Toggles menu visible and hidden
        /// </summary>
        protected void ToggleMenu(bool showLastUX = false)
        {
            menuOpen = !menuOpen;
            menu.visible = menuOpen;

            if (menu.visible == true)
            {
                uxManager.SetUXToNull();

                NonMenuBlockingUIElements = uiManager.blockingUIElements;
                uiDocument.rootVisualElement.Query<VisualElement>().ForEach(element =>
                {
                    if (element.ClassListContains("blockRaycast"))
                    {
                        uiManager.blockingUIElements.Add(element);
                    }
                });
            }
            else
            {
                if (showLastUX) uxManager.UseLastUX();
                uiManager.blockingUIElements = NonMenuBlockingUIElements;
            }

            uiManager.RegisterMouseDownCallback();
            uiManager.RegisterMouseUpCallback();
            uiManager.RegisterMouseLeaveCallback();
        }

        /// <summary>
        /// Adds a new menu item with an action to the menu
        /// </summary>
        /// <param name="item">The menu item to add</param>
        public void AddMenuItem(MenuItem item)
        {
            if (menuItems.Find(x => x.id == item.id) != null)
            {
                Debug.LogWarning("Menu item with id " + item.id + " already exists");
                return;
            }
            item.action += () =>
            {
                ToggleMenu(false);
            };
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