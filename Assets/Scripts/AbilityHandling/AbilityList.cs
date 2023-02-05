using System.Collections.Generic;
using UnityEngine;

namespace Pladdra
{
    [CreateAssetMenu(fileName = "Abilities List", menuName = "Pladdra/Abilities List", order = 1)]
    public class AbilityList : ScriptableObject
    {
        public List<Ability> abilities;
    }
}