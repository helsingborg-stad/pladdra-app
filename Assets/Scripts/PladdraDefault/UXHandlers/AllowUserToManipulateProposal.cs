using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.DefaultAbility.UX
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