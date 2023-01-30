using Pladdra.DialogueAbility.Data;
using UnityEngine;

namespace Pladdra.DialogueAbility
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Pladdra/Pladdra Default Settings", order = 1)]
    public class PladdraDefaultSettings : ScriptableObject
    {
        public float rotationSpeed = 1f;
        public AnimationCurve scaleCurve;
        public float previewObjectDistance;
        public GameObject objectPrefab;
    }
}