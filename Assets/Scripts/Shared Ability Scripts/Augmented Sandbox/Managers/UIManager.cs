using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.DialogueAbility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Pladdra.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        #region Public
        public List<UIObject> uiAssets = new List<UIObject>(); // ? Make some UI Assets required field
        [SerializeField] TextAsset uiTextAsset;
        [SerializeField] bool useSwedishLanguage = false;
        #endregion Public

        #region Scene References
        protected RaycastManager raycastManager { get { return transform.parent.gameObject.GetComponentInChildren<RaycastManager>(); } }
        protected MenuManager menuManager { get { return transform.parent.gameObject.GetComponentInChildren<MenuManager>(); } }
        public MenuManager MenuManager { get { return menuManager; } }
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        protected LoadingIndicator loadingIndicator { get { return GetComponent<LoadingIndicator>(); } }
        #endregion Scene References

        #region Events
        [HideInInspector] public UnityEvent onUpdatedUI = new UnityEvent();
        [HideInInspector] public UnityEvent onError = new UnityEvent();
        #endregion Events

        #region Hidden
        [HideInInspector] public List<VisualElement> blockingUIElements = new List<VisualElement>();
        #endregion Hidden

        #region Private
        Dictionary<string, string> uiTexts = new Dictionary<string, string>();
        string currentUI;
        string previousUI;
        Action<VisualElement> currentUIAction;
        Action<VisualElement> previousUIAction;
        #endregion Private

        void Start()
        {
            if (uiTextAsset != null)
            {
                // Populates our UI texts from the tsv file
                string[] t = uiTextAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                for (int i = 0; i < t.Length; i++)
                {
                    string[] t2 = t[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    uiTexts.Add(t2[0], (useSwedishLanguage && t2.Length == 3) ? t2[2] : t2[1]);
                }
            }

            //TODO Validate UI list so there are no duplicates
        }

        #region UI Controls

        /// <summary>
        /// Displays an UI element and assigns required actions
        /// </summary>
        /// <param name="uiName">The id of the UI element to display</param>
        /// <param name="bindUi">The actions to bind to the UI element</param>
        /// <param name="overrideAlreadyShown">If true, the UI will be displayed even if it is already shown</param>
        public void DisplayUI(string uiName, Action<VisualElement> bindUi = null, bool overrideAlreadyShown = false)
        {
            if (uiName == currentUI && !overrideAlreadyShown)
                return;

            previousUI = currentUI;
            previousUIAction = currentUIAction;

            currentUI = uiName;
            currentUIAction = bindUi;

            var uiObject = uiAssets.Find(ui => ui.name == uiName);
            if (uiObject == null)
            {
                ShowError($"UIManager: Could not find UI Asset with name {uiName}");
                return;
            }
            uiDocument.visualTreeAsset = uiObject.visualTreeAsset;
            var root = uiDocument.rootVisualElement;
            if (bindUi != null)
            {
                bindUi(root);
            }

            //Register Callbacks for raycast blocking UI Elements
            root.Query<VisualElement>().ForEach(element =>
            {
                if (element.ClassListContains("blockRaycast"))
                {
                    blockingUIElements.Add(element);
                    // Debug.Log("Adding element to blocking list: " + element.name + " in " + uiName);
                }
            });
            RegisterMouseDownCallback();
            RegisterMouseUpCallback();
            RegisterMouseLeaveCallback();

            onUpdatedUI.Invoke();
        }

        /// <summary>
        /// Removes the currently displayed UI.
        /// Using this will lead to no UI being displayed for the user, and no further interaction possible.
        /// </summary>
        public void ClearUI()
        {
            Debug.Log("Clearing UI");
            uiDocument.visualTreeAsset = null;
            currentUI = null;
        }

        /// <summary>
        /// Displays our most recent UI
        /// </summary>
        public void DisplayPreviousUI()
        {
            DisplayUI(previousUI, previousUIAction);
        }

        #endregion UI Controls

        /// <summary>
        /// Returns the currently displayed UI element.
        /// </summary>
        /// <returns>VisualElement</returns>
        public VisualElement GetCurrentUIElement()
        {
            return uiDocument.rootVisualElement;
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="errorText">Error text OR ID of the UI text to display in the error</param>
        /// <param name="args">The args to give that text</param>
        public void ShowError(string errorText, string[] args = null)
        {
            string errorTextToDisplay = args == null ? errorText : string.Format(uiTexts[errorText], args);
            Debug.LogError("Error: " + errorTextToDisplay);
            onError.Invoke();

            DisplayUI("error", root =>
            {
                var labelElement = root.Q<Label>("error-label");
                labelElement.text = errorTextToDisplay;
                root.Q<Button>("close").clicked += () =>
                {
                    DisplayPreviousUI();
                };
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
            DisplayUI("loading", root =>
                        {
                            root.Q<Label>("LoadingText").text = loadingText;
                        });

            if (loadingIndicator != null)
            {
                loadingIndicator.StartLoadingIndicator();
                onUpdatedUI.AddListener(() =>
                {
                    loadingIndicator.StopLoadingIndicator();
                    onUpdatedUI.RemoveListener(loadingIndicator.StopLoadingIndicator);
                });
            }
        }

        #region Block UI Raycasts

        //TODO This is not finished

        // Callback to be called when MouseDown on a panel.
        private void MouseDownCallback(MouseDownEvent evt)
        {
            raycastManager.isBlockedByUIElement = true;
        }

        // Callback to be called when MouseUp the panel.
        private void MouseUpCallback(MouseUpEvent evt)
        {
            raycastManager.isBlockedByUIElement = false;
        }

        // Callback to be called when MouseLeaves the panel.
        private void MouseLeaveCallback(MouseLeaveEvent evt)
        {
            raycastManager.isBlockedByUIElement = false;
        }

        // Method that registers the MouseDownCallback on the MouseDown UI event.
        public void RegisterMouseDownCallback()
        {
            // Registers a callback on the MouseDownEvent which will call MouseDownCallback
            if (blockingUIElements.Count != 0)
            {
                foreach (var element in blockingUIElements)
                {
                    // Debug.Log("Registering MouseDownCallback for element: " + element.name);
                    element.RegisterCallback<MouseDownEvent>(MouseDownCallback);
                }
            }
        }

        // Method that registers the MouseUpCallback on the MouseUp UI event.
        public void RegisterMouseUpCallback()
        {
            // Registers a callback on the MouseUpEvent which will call MouseUpCallback
            if (blockingUIElements.Count != 0)
            {
                foreach (var element in blockingUIElements)
                {
                    // Debug.Log("Registering MouseUpCallback for element: " + element.name);
                    element.RegisterCallback<MouseUpEvent>(MouseUpCallback);
                }
            }
        }

        // Method that registers the MouseLeaveCallback on the MouseUp UI event.
        public void RegisterMouseLeaveCallback()
        {
            // Registers a callback on the MouseLeaveEvent which will call MouseLeaveCallback
            if (blockingUIElements.Count != 0)
            {
                foreach (var element in blockingUIElements)
                {
                    // Debug.Log("Registering MouseLeaveCallback for element: " + element.name);
                    element.RegisterCallback<MouseLeaveEvent>(MouseLeaveCallback);
                }
            }
        }
        #endregion Block UI Raycasts
    }

    /// <summary>
    /// Object to reference UI elements
    /// </summary>
    [System.Serializable]
    public class UIObject
    {
        public string name;
        public VisualTreeAsset visualTreeAsset;
    }
}