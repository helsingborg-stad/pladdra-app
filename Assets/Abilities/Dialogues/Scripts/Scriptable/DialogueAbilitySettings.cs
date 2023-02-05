using Pladdra.ARSandbox.Dialogues.Data;
using UnityEngine;

namespace Pladdra.ARSandbox.Dialogues
{
    [CreateAssetMenu(fileName = "DialogueAbilitySettings", menuName = "Pladdra/Dialogue Ability Settings", order = 1)]
    public class DialogueAbilitySettings : ScriptableObject
    {
        public float rotationSpeed = 1f;
        public AnimationCurve scaleCurve;
        public float previewObjectDistance;
        public GameObject pivotPrefab;
        public GameObject objectPrefab;
    }
}