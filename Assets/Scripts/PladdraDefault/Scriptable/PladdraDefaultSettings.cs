using Pladdra.DefaultAbility.Data;
using UnityEngine;

namespace Pladdra.DefaultAbility
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Pladdra Default/Settings", order = 1)]
    public class PladdraDefaultSettings : ScriptableObject
    {
        public float rotationSpeed = 1f;
        public AnimationCurve scaleCurve;
        public bool useTestProjects = false;
        public LocalTestProjectList localProjectList;
        public ProjectList projectList;
    }
}