using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Common;
using UnityEngine;
using Pladdra.Workspace;
using Object = UnityEngine.Object;

namespace UXHandlers
{
    public abstract class AbstractUxHandler : IUxHandler, IUxHandlerEvents
    {
        protected AbstractUxHandler(): this(Traits.AllowTranslate, Traits.AllowRotate, Traits.AllowScale, Traits.AllowBoxCollider, Traits.AllowFlexibleBounds, Traits.AllowSelect, Traits.AllowOutline){}
        protected AbstractUxHandler(params IUxHandlerTrait[] traits)
        {
            UxHandlerTraits = traits.ToList();
        }

        protected List<IUxHandlerTrait> UxHandlerTraits { get; set; }

        protected abstract IEnumerable<GameObject> GetSelectableObjects(IWorkspaceScene scene);

        public virtual void OnSelected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            InvokeTraits(go, scene, workspace, (trait, ctx) => trait.Select(ctx));
        }

        public virtual void OnDeselected(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            InvokeTraits(go, scene, workspace, (trait, ctx) => trait.Deselect(ctx));
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
            InvokeTraits(go, scene, workspace, (trait, ctx) => trait.Activate(ctx));
        }

        protected virtual void DeactivateObject(IWorkspaceScene scene, IWorkspace workspace, GameObject go)
        {
            InvokeTraits(go, scene, workspace, (trait, ctx) => trait.Deactivate(ctx));
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
        private IUxHandlerTraitContext CreateTraitContext(GameObject go, IWorkspaceScene scene, IWorkspace workspace)
        {
            return new TraitContext(this, go, scene, workspace);
        }

        private class TraitContext : IUxHandlerTraitContext
        {
            public TraitContext(IUxHandlerEvents events,  GameObject go, IWorkspaceScene scene, IWorkspace workspace)
            {
                Events = events;
                GameObject = go;
                Scene = scene;
                Workspace = workspace;
            }

            public GameObject GameObject { get; }
            public IWorkspace Workspace { get; }
            public IWorkspaceScene Scene { get; }
            public IUxHandlerEvents Events { get; }
            public TComponent TryConfigureComponent<TComponent>(Action<TComponent> configure)
            {
                var component = default(TComponent);
                if (GameObject.TryGetComponent<TComponent>(out component))
                {
                    configure(component);
                }
                return component;
            }
        }

        protected virtual void InvokeTraits(GameObject go,
            IWorkspaceScene scene, IWorkspace workspace,
            Action<IUxHandlerTrait, IUxHandlerTraitContext> invoke)
        {
            var ctx = CreateTraitContext(go, scene, workspace);
            UxHandlerTraits.ForEach(t => invoke(t, ctx));
        }
    }
}