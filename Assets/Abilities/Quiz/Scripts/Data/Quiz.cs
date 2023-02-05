using System.Collections.Generic;

namespace Pladdra.ARSandbox.Quizzes.Data
{
    [System.Serializable]
    public class Quiz
    {
        public Quiz(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.questions = new List<Question>();
        }
        public string name;
        public string description;
        public List<Question> questions;
    }
}
