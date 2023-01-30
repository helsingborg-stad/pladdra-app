namespace Pladdra.QuizAbility.Data
{
    [System.Serializable]
    public class Answer
    {
        public Answer(string text, bool isCorrect)
        {
            this.text = text;
            this.isCorrect = isCorrect;
        }
        public string text;
        public bool isCorrect;
    }
}