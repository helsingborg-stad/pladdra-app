using Pladdra.DialogueAbility.Data;
using UnityEngine;

namespace Pladdra.DialogueAbility
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Pladdra/Dialogue Ability Settings", order = 1)]
    public class DialogueAbilitySettings : ScriptableObject
    {
        public float rotationSpeed = 1f;
        public AnimationCurve scaleCurve;
        public float previewObjectDistance;
        public GameObject pivotPrefab;
        public GameObject objectPrefab;
    }
}