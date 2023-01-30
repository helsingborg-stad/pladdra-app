using System.Collections.Generic;
using UnityEngine;

namespace Pladdra
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Pladdra/Abilities List", order = 1)]
    public class AbilityList : ScriptableObject
    {
        public List<Ability> abilities;
    }
}