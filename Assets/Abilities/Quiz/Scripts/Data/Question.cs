using System.Collections.Generic;

namespace Pladdra.ARSandbox.Quizzes.Data
{
    [System.Serializable]
    public class Question
    {
        public Question(string text, string response)
        {
            this.text = text;
            this.response = response;
            this.answers = new List<Answer>();
        }
        public string text;
        public string response;
        public List<Answer> answers;
        public bool answered;
    }
}