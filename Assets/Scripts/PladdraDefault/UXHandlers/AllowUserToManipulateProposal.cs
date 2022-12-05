using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToManipulateProposal : AllowUserToManipulateWorkspace
    {
        public AllowUserToManipulateProposal(InteractionManager interactionManager) : base(interactionManager)
        {
        }
        protected override void Return()
        {
            UXHandler ux = new AllowUserToViewProposal(interactionManager);
            interactionManager.UseUxHandler(ux);
        }
        
    }
}