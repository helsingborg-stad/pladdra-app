using UnityEngine;
using UnityEngine.UIElements;
using Pladdra.ARSandbox.Dialogues.UX;
using Pladdra;
using Pladdra.UI;
using Pladdra.ARSandbox.Dialogues;
using Pladdra.ARSandbox.Dialogues.Data;
using Pladdra.ARSandbox;

namespace Pladdra.UX
{
    /// <summary>
    /// Manages UXHandlers.
    /// </summary>
    public interface IUXManager
    {
        public IUXHandler uxHandler { get; set; }
        public IUXHandler pastUXHandler { get; set; }
        public GameObject User { get; set; }
        void UseUxHandler(IUXHandler uxHandler);
        void SetUXToNull();
        void UseLastUX();
        void ShowWorkspaceDefault();
        void SelectObject(GameObject obj);

    }
}