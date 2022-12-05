using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        #region Public
        //TODO Add validate that there are no duplicates in the list
        public List<UIObject> uiAssets = new List<UIObject>(); // ? Make some UI Assets required field
        public VisualTreeAsset buttonTemplate;
        public VisualTreeAsset projectButtonTemplate;
        public VisualTreeAsset resourceButtonTemplate;
        public VisualTreeAsset proposalButtonTemplate;
        public TextAsset uiTextAsset;
        public bool useSwedishLanguage = false;
        #endregion Public

        #region Private
        protected MenuManager menuManager { get { return transform.parent.gameObject.GetComponentInChildren<MenuManager>(); } }
        public MenuManager MenuManager { get { return menuManager; } }
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        protected LoadingIndicator loadingIndicator { get { return GetComponent<LoadingIndicator>(); } }
        [HideInInspector] UnityEvent updatedUI = new UnityEvent();
        Dictionary<string, string> uiTexts = new Dictionary<string, string>();
        string currentUI;
        string previousUI;
        Action<VisualElement> currentUIAction;
        Action<VisualElement> previousUIAction;
        #endregion Private
        void Start()
        {
            // Populates our UI texts from the tsv file
            string[] t = uiTextAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < t.Length; i++)
            {
                string[] t2 = t[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                uiTexts.Add(t2[0], (useSwedishLanguage && t2.Length == 3) ? t2[2] : t2[1]);
            }
        }

        /// <summary>
        /// Shows our last shown UI
        /// </summary>
        public void ShowPreviousUI()
        {
            Debug.Log("Showing previous UI: " + previousUI);
            ShowUI(previousUI, previousUIAction);
        }

        /// <summary>
        /// Shows an UI element and assigns required actions
        /// </summary>
        /// <param name="uiName">The id of the UI element to show</param>
        /// <param name="bindUi">The actions to bind to the UI element</param>
        public void ShowUI(string uiName, Action<VisualElement> bindUi = null)
        {
            Debug.Log("UIManager: ShowUI " + uiName);
            if(uiName == currentUI)
            {
                Debug.Log("UIManager: UI already shown");
                return;
            }
            previousUI = currentUI;
            previousUIAction = currentUIAction;

            currentUI = uiName;
            currentUIAction = bindUi;

            var uiObject = uiAssets.Find(ui => ui.name == uiName);
            if (uiObject == null)
            {
                Debug.LogError($"UIManager: Could not find UI Asset with name {uiName}");
                //TODO: Show error screen
                return;
            }
            uiDocument.visualTreeAsset = uiObject.visualTreeAsset;
            var root = uiDocument.rootVisualElement;
            if (bindUi != null)
            {
                bindUi(root);
            }

            updatedUI.Invoke();
        }

        /// <summary>
        /// Removes the currently displayed UI.
        /// Using this will lead to no UI being displayed for the user, and no further interaction possible.
        /// </summary>
        public void ClearUI()
        {
            Debug.Log("Clearing UI");
            uiDocument.visualTreeAsset = null;
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="uiText">ID of the UI text to display in the error</param>
        /// <param name="args">The args to give that text</param>
        public void ShowError(string uiText, string[] args)
        {
            Debug.Log($"Showing error {uiText}");
            ShowUI("error", root =>
            {
                var labelElement = root.Q<Label>("error-label");
                labelElement.text = string.Format(uiTexts[uiText], args);
            });
        }

        //TODO Add error actions
        //TODO Change loading text to UItext
        /// <summary>
        /// Initiates the loading icon
        /// </summary>
        /// <param name="loadingText"></param>
        internal void ShowLoading(string loadingText)
        {
            ShowUI("loading", root =>
                        {
                            root.Q<Label>("LoadingText").text = loadingText;
                        });

            if (loadingIndicator != null)
            {
                loadingIndicator.StartLoadingIndicator();
                updatedUI.AddListener(() =>
                {
                    loadingIndicator.StopLoadingIndicator();
                    updatedUI.RemoveListener(loadingIndicator.StopLoadingIndicator);
                });
            }
        }
    }

    /// <summary>
    /// Object to hold UI elements
    /// </summary>
    [System.Serializable]
    public class UIObject
    {
        public string name;
        public VisualTreeAsset visualTreeAsset;
    }
}