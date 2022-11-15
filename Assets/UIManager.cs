using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Pladdra.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        #region Public
        //TODO Validate that there are no duplicates in the list
        public List<UIObject> uiAssets = new List<UIObject>(); // ? Make some UI Assets required field, but this might need a custom drawer
        public VisualTreeAsset buttonTemplate;
        public VisualTreeAsset projectButtonTemplate;
        public VisualTreeAsset resourceButtonTemplate;
        public TextAsset uiTextAsset;
        public bool useSwedishLanguage = false;
        #endregion Public

        #region Private
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }
        protected LoadingIndicator loadingIndicator { get { return GetComponent<LoadingIndicator>(); } }
        [HideInInspector] UnityEvent updatedUI = new UnityEvent();
        Dictionary<string, string> uiTexts = new Dictionary<string, string>();
        string currentUI;
        string previousUI;
        #endregion Private
        void Start()
        {
            // split uiTextAsset by new line
            string[] t = uiTextAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for(int i = 0; i < t.Length; i++)
            {
                // split t[i] by tab and add to uiTexts
                string[] t2 = t[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                uiTexts.Add(t2[0], (useSwedishLanguage && t2.Length == 3) ? t2[2] : t2[1]);
            }
            Debug.Log("UIManager: Loaded " + uiTexts.Count + " UI texts");
        }

        public void ShowPreviousUI()
        {
            ShowUI(previousUI);
        }

        public void ShowUI(string uiName, Action<VisualElement> bindUi = null)
        {
            previousUI = currentUI;
            currentUI = uiName;

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

        public void ClearUI()
        {
            Debug.Log("Clearing UI");
            uiDocument.visualTreeAsset = null;
        }

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


    [System.Serializable]
    public class UIObject
    {
        public string name;
        public VisualTreeAsset visualTreeAsset;
    }
}