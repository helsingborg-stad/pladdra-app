namespace UXHandlers
{
    public interface IUxHandlerTrait
    {
        void Activate(IUxHandlerTraitContext context);
        void Deactivate(IUxHandlerTraitContext context);
        void Select(IUxHandlerTraitContext context);
        void Deselect(IUxHandlerTraitContext context);
    }
}