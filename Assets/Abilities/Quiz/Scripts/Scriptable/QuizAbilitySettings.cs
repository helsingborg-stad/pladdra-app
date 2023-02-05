using Pladdra.ARSandbox.Dialogues.Data;
using UnityEngine;

namespace Pladdra.ARSandbox.Quizzes
{
    [CreateAssetMenu(fileName = "QuizAbilitySettings", menuName = "Pladdra/Quiz Ability Settings", order = 1)]
    public class QuizAbilitySettings : ScriptableObject
    {
        public GameObject answerPrefab;
        public Material wrongAnswer;
        public Material correctAnswer;
        public Material disabledAnswer;
        public float distance = 2;
        public float range = 1;
    }
}