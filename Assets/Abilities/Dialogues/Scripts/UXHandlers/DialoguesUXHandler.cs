using Pladdra.ARSandbox.Dialogues.Data;
using Pladdra.UX;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public abstract class DialoguesUXHandler : IUXHandler
    {
        protected Project project;
        protected DialoguesUXManager uxManager;
        public abstract void Activate();
        public abstract void Deactivate();  

    }
}