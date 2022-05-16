using System;
using UnityEngine;

namespace Screens
{
    public abstract class Screen : MonoBehaviour
    {
        protected virtual object[] GetScreenChildObjects() { return Array.Empty<object>(); }

        protected virtual void BeforeActivateScreen() { }
        protected virtual void AfterActivateScreen() { }
        protected virtual void BeforeDeactivateScreen() { }
        protected virtual void AfterDeactivateScreen() { }


        public void SetScreenActive(bool active)
        {
            if (active)
            {
                BeforeActivateScreen();
            }
            else
            {
                BeforeDeactivateScreen();
            }
            gameObject.SetActive(active);

            foreach (var child in GetScreenChildObjects())
            {
                var childScreen = child as Screen;
                if (childScreen)
                {
                    childScreen.SetScreenActive(active);
                    continue;
                }
                var childMonoBehaviour = child as MonoBehaviour;
                if (childMonoBehaviour)
                {
                    childMonoBehaviour.gameObject.SetActive(active);
                    continue;
                }
                var childGameObject = child as GameObject;
                if (childGameObject)
                {
                    childGameObject.SetActive(active);
                    continue;
                }
            }
            if (active)
            {
                AfterActivateScreen();
            }
            else
            {
                AfterDeactivateScreen();
            }
        }
    }
}