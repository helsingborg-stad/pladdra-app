using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;

namespace Pladdra.DefaultAbility.UX
{
    public abstract class UXHandler
    {
        protected Project project;
        protected UXManager uxManager;
        public abstract void Activate();
        public abstract void Deactivate();  

    }
}