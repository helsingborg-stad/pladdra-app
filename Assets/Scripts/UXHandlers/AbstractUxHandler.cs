using System;
using System.Collections.Generic;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using Utility;
using Workspace;
using Object = UnityEngine.Object;

namespace UXHandlers
{
    public abstract class AbstractUxHandler : IUxHandler
    {
        protected abstract IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene);

        protected virtual void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go) {}
        protected virtual void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go) {}
        
        public virtual void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var obj in GetSelectableObjects(scene))
            {
                TryConfigureComponent<LeanDragTranslateAlong>(obj, c => c.enabled = true);
                TryConfigureComponent<LeanTwistRotateAxis>(obj, c => c.enabled = true);
                TryConfigureComponent<LeanPinchScale>(obj, c => c.enabled = true);
                TryConfigureComponent<BoxCollider>(obj, c => c.enabled = true);
                TryConfigureComponent<LeanSelectable>(obj, selectable =>
                {
                    selectable.enabled = true;
                    selectable.OnSelected.AddListener((leanSelect) => OnSelected(scene, workspace, obj));
                    selectable.OnDeselected.AddListener((leanSelect) => OnDeselected(scene, workspace, obj));
                });
                
                TryConfigureComponent<FlexibleBounds>(obj, c =>
                {
                    c.CalculateBoundsFromChildrenAndThen(obj, bounds =>
                    {
                        TryConfigureComponent<BoxCollider>(obj, boxCollider =>
                        {
                            boxCollider.center = bounds.center;
                            boxCollider.size = bounds.size;
                        });
                        
                        TryConfigureComponent<MeshFilter>(obj, meshFilter =>
                        {
                            meshFilter.mesh = new CubeFactory(bounds.size, bounds.center).CreateMesh();
                        });
                    });
                });
            }
        }

        public virtual void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var obj in GetSelectableObjects(scene))
            {
                TryConfigureComponent<LeanDragTranslateAlong>(obj, c => c.enabled = false);
                TryConfigureComponent<LeanTwistRotateAxis>(obj, c => c.enabled = false);
                TryConfigureComponent<LeanPinchScale>(obj, c => c.enabled = false);
                TryConfigureComponent<BoxCollider>(obj, c => c.enabled = false);
                TryConfigureComponent<LeanSelectable>(obj, selectable =>
                {
                    selectable.enabled = false;
                    selectable.OnSelected.RemoveAllListeners();
                    selectable.OnDeselected.RemoveAllListeners();
                });
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

        protected T TryConfigureComponent<T>(GameObject go, Action<T> configure) where T : class
        {
            var component = default(T);
            if (go.TryGetComponent<T>(out component))
            {
                configure(component);
            }

            return component;
        }  
    }
}