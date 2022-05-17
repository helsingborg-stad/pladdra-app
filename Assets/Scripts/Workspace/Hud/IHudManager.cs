using System;
using UnityEngine.UIElements;

namespace Workspace.Hud
{
    public interface IHudManager
    {
        void UseHud(string templatePath, Action<VisualElement> bindUi);
        void ClearHud();
    }
}