using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Workspace.UxHandlers.ObjectInspectors
{
    public abstract class AbstractGameObjectInspector<TModel>
    {
        private readonly GameObject gameObject;
        private readonly VisualElement root;
        private List<Action<TModel>> updaters = new List<Action<TModel>>();
        protected abstract TModel GetModel(GameObject gameObject);
        protected abstract void SetModel(GameObject gameObject, TModel model);

        protected AbstractGameObjectInspector(GameObject gameObject, VisualElement root)
        {
            this.gameObject = gameObject;
            this.root = root;
        }
        protected void AddTextField(string name, Func<TModel, float> mapField, Func<float, TModel, TModel> updateModel)
        {
            var textField = root.Q<TextField>(name);
            if (textField != null)
            {
                textField.value = mapField(GetModel(gameObject)).ToString();
                textField?.RegisterValueChangedCallback(e =>
                {
                    float v;
                    if (float.TryParse(e.newValue, out v))
                    {
                        SetModel(gameObject, updateModel(v, GetModel(gameObject)));
                    }
                });
                updaters.Add(model => textField.value = mapField(model).ToString());
            }
        }

        protected void UpdateUI(TModel model)
        {
            foreach (var updater in updaters)
            {
                updater(model);
            }
        }
    }
}