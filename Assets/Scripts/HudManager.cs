using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class HudManager : MonoBehaviour
    {
        public void UseHud(string templatePath, Action<VisualElement> bindUi)
        {
            var uxml = Resources.Load<VisualTreeAsset>(templatePath);
            var uiDocument = FindObjectOfType<UIDocument>();
            uiDocument.visualTreeAsset = uxml;
            var root = uiDocument.rootVisualElement;
            bindUi(root);
        }

        public void ClearHud()
        {
            FindObjectOfType<UIDocument>().visualTreeAsset = null;
        }
    }
}