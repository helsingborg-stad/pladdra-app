using System;
using System.Collections.Generic;
using System.Linq;
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

        protected virtual void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            TryConfigureComponent<MeshRenderer>(go, c =>
            {
                c.materials
                    .Where(material => material.shader.name == "Sprites/Outline")
                    .ToList()
                    .ForEach(material =>
                    {
                        material.SetColor("_Color", new Color(1f, 0f, 0f, 0.12f));
                        material.SetColor("_SolidOutline", new Color(1f, 0f, 0f, 0.66f));
                    });
            });
        }

        protected virtual void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            TryConfigureComponent<MeshRenderer>(go, c =>
            {
                c.materials
                    .Where(material => material.shader.name == "Sprites/Outline")
                    .ToList()
                    .ForEach(material =>
                    {
                        material.SetColor("_Color", new Color(1f, 1f, 1f, 0.16f));
                        material.SetColor("_SolidOutline", new Color(1f, 1f, 1f, 0.66f));
                    });
            });
        }
        
        public virtual void Activate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var go in GetSelectableObjects(scene))
            {
                ActivateObject(scene, workspace, go);
            }
        }

        public virtual void Deactivate(IWorkspaceScene scene, IWorkspace workspace)
        {
            foreach (var go in GetSelectableObjects(scene))
            {
                DeactivateObject(scene, workspace, go);
            }
        }

        protected virtual void ActivateObject(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
                TryConfigureComponent<LeanDragTranslateAlong>(go, c => c.enabled = true);
                TryConfigureComponent<LeanTwistRotateAxis>(go, c => c.enabled = true);
                TryConfigureComponent<LeanPinchScale>(go, c => c.enabled = true);
                TryConfigureComponent<BoxCollider>(go, c => c.enabled = true);
                TryConfigureComponent<LeanSelectable>(go, selectable =>
                {
                    selectable.enabled = true;
                    selectable.OnSelected.AddListener((leanSelect) => OnSelected(scene, workspace, go));
                    selectable.OnDeselected.AddListener((leanSelect) => OnDeselected(scene, workspace, go));
                });
                
                TryConfigureComponent<FlexibleBounds>(go, c =>
                {
                    c.CalculateBoundsFromChildrenAndThen(go, bounds =>
                    {
                        TryConfigureComponent<BoxCollider>(go, boxCollider =>
                        {
                            boxCollider.center = bounds.center;
                            boxCollider.size = bounds.size;
                        });
                        
                        TryConfigureComponent<MeshFilter>(go, meshFilter =>
                        {
                            meshFilter.mesh = new BoundingBoxFactory(bounds.size, bounds.center).CreateMesh();
                        });
                    });
                });
                
                TryConfigureComponent<MeshRenderer>(go, c =>
                {
                    c.enabled = true;
                    c.materials
                        .Where(material => material.shader.name == "Sprites/Outline")
                        .ToList()
                        .ForEach(material =>
                        {
                            material.SetColor("_Color", new Color(1f, 1f, 1f, 0.16f));
                            material.SetColor("_SolidOutline", new Color(1f, 1f, 1f, 0.66f));
                        });
                });
        }

        protected virtual void DeactivateObject(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            TryConfigureComponent<LeanDragTranslateAlong>(go, c => c.enabled = false);
            TryConfigureComponent<LeanTwistRotateAxis>(go, c => c.enabled = false);
            TryConfigureComponent<LeanPinchScale>(go, c => c.enabled = false);
            TryConfigureComponent<BoxCollider>(go, c => c.enabled = false);
            TryConfigureComponent<LeanSelectable>(go, selectable =>
            {
                selectable.enabled = false;
                selectable.OnSelected.RemoveAllListeners();
                selectable.OnDeselected.RemoveAllListeners();
            });
                
            TryConfigureComponent<MeshRenderer>(go, c =>
            {
                c.enabled = false;
            });

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