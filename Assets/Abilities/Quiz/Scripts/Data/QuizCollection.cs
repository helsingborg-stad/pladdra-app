using System.Collections.Generic;

namespace Pladdra.QuizAbility.Data
{
    [System.Serializable]
    public class QuizCollection
    {
        public QuizCollection(string id, string name, string description, string title, string info)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.title = title;
            this.info = info;
            this.quizzes = new List<Quiz>();
            this.isLoadedAndInit = false;
        }
        public string id;
        public string name;
        public string description;
        public string title;
        public string info;
        public List<Quiz> quizzes;
        public bool isLoadedAndInit;
    }
}