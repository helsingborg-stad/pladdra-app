using System;

namespace UXHandlers
{
    public class UxHandlerTrait<TComponent> : IUxHandlerTrait
    {
        public Action<TComponent, IUxHandlerTraitContext> OnActivate { get; set; }
        public Action<TComponent, IUxHandlerTraitContext> OnDeactivate { get; set; }
        public Action<TComponent, IUxHandlerTraitContext> OnSelect { get; set; }
        public Action<TComponent, IUxHandlerTraitContext> OnDeselect { get; set; }

        public void Activate(IUxHandlerTraitContext context)
        {
            TryInvoke(context, OnActivate);
        }

        public void Deactivate(IUxHandlerTraitContext context)
        {
            TryInvoke(context, OnDeactivate);
        }

        public void Select(IUxHandlerTraitContext context)
        {
            TryInvoke(context, OnSelect);
        }

        public void Deselect(IUxHandlerTraitContext context)
        {
            TryInvoke(context, OnDeselect);
        }

        private void TryInvoke(IUxHandlerTraitContext context, Action<TComponent, IUxHandlerTraitContext> action)
        {
            if (action != null)
            {
                var component = default(TComponent);
                if (context.GameObject.TryGetComponent<TComponent>(out component))
                {
                    action(component, context);
                }
            }
        }  

    }
}