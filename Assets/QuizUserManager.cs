using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.QuizAbility.Data;
using System.Linq;
using Pladdra.UI;
using UnityEngine.UIElements;

namespace Pladdra.QuizAbility
{
    public class QuizUserManager : MonoBehaviour
    {
        [GrayOut] public User user;

        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }

        void Start()
        {
            user = new User("1", "Name");
        }

        public void AddQuiz(Quiz quiz)
        {
            user.AddQuiz(quiz);
        }

        public void UpdateScore()
        {
            user.currentQuiz.score++;
        }
        public int GetScore()
        {
            return user.currentQuiz.score;
        }
    }


}