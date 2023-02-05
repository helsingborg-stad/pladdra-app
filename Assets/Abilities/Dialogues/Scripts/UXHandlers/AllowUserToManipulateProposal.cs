using Pladdra.UX;

namespace Pladdra.ARSandbox.Dialogues.UX
{
    public class AllowUserToManipulateProposal : AllowUserToManipulateWorkspace
    {
        public AllowUserToManipulateProposal(DialoguesUXManager uxManager) : base(uxManager)
        {
        }
        protected override void Return()
        {
            IUXHandler ux = new AllowUserToViewProposal(uxManager);
            uxManager.UseUxHandler(ux);
        }
        
    }
}