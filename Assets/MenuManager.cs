using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuManager : MonoBehaviour
    {
        UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        AppManager appManager { get { return transform.parent.gameObject.GetComponentInChildren<AppManager>(); } }
        Button menuButton;
        VisualElement menu;
        bool menuOpen;
        void Start()
        {
            menu = uiDocument.rootVisualElement.Q<VisualElement>("menu");
            menu.visible = false;

            uiDocument.rootVisualElement.Q<Button>("menu-button").clicked += () =>
            {
                ToggleMenu();
            };

            uiDocument.rootVisualElement.Q<Button>("project-list").clicked += () =>
            {
                appManager.ShowTestProjectList();
                ToggleMenu();
            };
            uiDocument.rootVisualElement.Q<Button>("about").clicked += () =>
            {
                uiManager.ShowUI("about-pladdra", root =>
                        {
                            root.Q<Button>("close").clicked += () => { uiManager.ShowPreviousUI(); };
                        });
                ToggleMenu();
            };
            uiDocument.rootVisualElement.Q<Button>("contact").clicked += () =>
            {
                Application.OpenURL("mailto:jakob@untold.garden");
                ToggleMenu();
            };
        }

        void ToggleMenu()
        {
            menuOpen = !menuOpen;
            menu.visible = menuOpen;
        }
    }
}