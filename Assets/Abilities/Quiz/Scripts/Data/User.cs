using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.QuizAbility.Data
{
    [System.Serializable]
    public class User
    {
        public User(string id, string name)
        {
            this.id = id;
            this.name = name;
            responses = new List<Responses>();
        }
        public string id;
        public string name;
        public List<Responses> responses;
        public Responses currentQuiz;

        public void AddQuiz(Quiz quiz)
        {
            if (responses == null)
            {
                responses = new List<Responses>();
            }
            if (responses.Find(x => x.quiz == quiz) == null)
            {
                currentQuiz = new Responses(quiz);
                responses.Add(currentQuiz);
            }
            else
            {
                currentQuiz = responses.Find(x => x.quiz == quiz);
                currentQuiz.score = 0;
            }
        }
    }

    [System.Serializable]
    public class Responses
    {
        public Responses(Quiz quiz)
        {
            this.quiz = quiz;
        }
        public Quiz quiz;
        public int score;
    }
}