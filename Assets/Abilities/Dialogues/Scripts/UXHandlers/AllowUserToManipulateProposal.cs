using Pladdra.UX;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToManipulateProposal : AllowUserToManipulateWorkspace
    {
        public AllowUserToManipulateProposal(UXManager uxManager) : base(uxManager)
        {
        }
        protected override void Return()
        {
            UXHandler ux = new AllowUserToViewProposal(uxManager);
            uxManager.UseUxHandler(ux);
        }
        
    }
}