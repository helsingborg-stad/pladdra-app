using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



namespace Pladdra
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        //TODO Validate that there are no duplicates in the list
        public List<UIObject> uiAssets = new List<UIObject>(); // ? Make some UI Assets required field, but this might need a custom drawer
        protected UIDocument uiDocument { get { return GetComponent<UIDocument>(); } }

        public void ShowUI(string uiName, Action<VisualElement> bindUi = null)
        {
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
        }

        public void ClearUI()
        {
            Debug.Log("Clearing UI");
            uiDocument.visualTreeAsset = null;
        }

        public void ShowError(string errorMessage = "An error has occured")
        {
            Debug.Log($"Showing error {errorMessage}");
            ShowUI("error", root =>
            {
                var labelElement = root.Q<Label>("error-label"); 
                labelElement.text = errorMessage;
            });
        }
    }

    [System.Serializable]
    public class UIObject
    {
        public string name;
        public VisualTreeAsset visualTreeAsset;
    }
}
