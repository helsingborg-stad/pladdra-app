using System.Collections.Generic;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using Workspace;

namespace UXHandlers
{
    public abstract class AbstractUxHandler : IUxHandler
    {
        protected abstract IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene);

        protected virtual void OnSelected(IWorkspaceScene scene, GameObject go) {}
        protected virtual void OnDeselected(IWorkspaceScene scene, GameObject go) {}
        
        public virtual void Activate(IWorkspaceScene scene)
        {
            foreach (var obj in GetSelectableObjects(scene))
            {
                obj.GetComponent<LeanDragTranslateAlong>().enabled = true;
                obj.GetComponent<LeanTwistRotateAxis>().enabled = true;
                obj.GetComponent<BoxCollider>().enabled = true;
                obj.GetComponent<FlexibleBoxCollider>().SetBoxColliderSize();

                var selectable = obj.GetComponent<LeanSelectable>();
                selectable.enabled = true;
                selectable.OnSelected.AddListener(() => OnSelected(scene, obj));
                selectable.OnDeselected.AddListener(() => OnDeselected(scene, obj));
            }
        }

        public virtual void Deactivate(IWorkspaceScene scene)
        {
            foreach (var obj in GetSelectableObjects(scene))
            {
                obj.GetComponent<LeanDragTranslateAlong>().enabled = false;
                obj.GetComponent<LeanTwistRotateAxis>().enabled = false;
                obj.GetComponent<BoxCollider>().enabled = false;

                var selectable = obj.GetComponent<LeanSelectable>();
                selectable.enabled = false;
                selectable.OnSelected.RemoveAllListeners();
                selectable.OnDeselected.RemoveAllListeners();
            }
        }

        protected void SelectObject(GameObject go)
        {
            Object.FindObjectOfType<LeanSelect>().Select(go.GetComponent<LeanSelectable>());
        }
        protected void DeselectAll()
        {
            Object.FindObjectOfType<LeanSelect>().DeselectAll();
        }
    }
}