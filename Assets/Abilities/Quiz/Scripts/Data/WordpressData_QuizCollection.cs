using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Data;
using System;

namespace Pladdra.QuizAbility.Data
{
    [System.Serializable]
    public class WordpressData_QuizCollection : WordpressData
    {
        public Acf acf;

        public QuizCollection MakeQuizCollection(out string error)
        {
            error = "";
            QuizCollection quizCollection = new QuizCollection(id, acf.settings.name, acf.settings.description, acf.information.title, acf.information.information);
            foreach (WordpressData_QuizGroup quizGroup in acf.quizzes)
            {
                if (quizGroup.quiz.questions == null)
                    continue;
                Quiz quiz = new Quiz(quizGroup.quiz.name, quizGroup.quiz.description);
                foreach (WordpressData_QuestionGroup questionGroup in quizGroup.quiz.questions)
                {
                    Question question = new Question(questionGroup.question.text, questionGroup.question.response);
                    foreach (WordpressData_AnswerGroup answerGroup in questionGroup.question.answers)
                    {
                        bool correct = false;
                        try
                        {
                            correct = Convert.ToBoolean(answerGroup.answer.correct);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Error parsing question bool:" + e);
                        }
                        question.answers.Add(new Answer(answerGroup.answer.text, correct));
                    }
                    quiz.questions.Add(question);
                }
                quizCollection.quizzes.Add(quiz);
            }
            if (quizCollection.quizzes.Count == 0)
            {
                error = "No quizzez";
                return null;
            }
            else
                return quizCollection;
        }
    }

    [System.Serializable]
    public class Acf
    {
        public Settings settings;
        public QuizInformation information;
        public WordpressData_QuizGroup[] quizzes;
    }

    [System.Serializable]
    public class Settings
    {
        public string name;
        public string description;
    }

    [System.Serializable]
    public class QuizInformation
    {
        public string title;
        public string information;
    }

    /// <summary>
    /// This annoying middle layer is necessary since we use the Wordpress group feature to make a more user friendly interface.
    /// </summary>
    [System.Serializable]
    public class WordpressData_QuizGroup
    {
        public WordpressData_Quiz quiz;
    }
    [System.Serializable]
    public class WordpressData_Quiz
    {
        public string name;
        public string description;
        public WordpressData_QuestionGroup[] questions;
    }

    /// <summary>
    /// This annoying middle layer is necessary since we use the Wordpress group feature to make a more user friendly interface.
    /// </summary>
    [System.Serializable]
    public class WordpressData_QuestionGroup
    {
        public WordpressData_Question question;
    }
    [System.Serializable]
    public class WordpressData_Question
    {
        public string text;
        public string response;
        public WordpressData_AnswerGroup[] answers;
    }

    /// <summary>
    /// This annoying middle layer is necessary since we use the Wordpress group feature to make a more user friendly interface.
    /// </summary>
    [System.Serializable]
    public class WordpressData_AnswerGroup
    {
        public WordpressData_Answer answer;
    }

    [System.Serializable]
    public class WordpressData_Answer
    {
        public string text;
        public string correct;
    }
}