using Pladdra.DialogueAbility.Data;

namespace Pladdra.UX
{
    public abstract class UXHandler
    {
        protected Project project;
        protected UXManager uxManager;
        public abstract void Activate();
        public abstract void Deactivate();  

    }
}