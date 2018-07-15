namespace Assets.Scripts.QuizGame
{
    [System.Serializable]
    public class QuestionData
    {
        // Completely redundant at this point since I added the question to the AnswerData class.
        // Leaving it here for now in case I decide to do something with it later once the category's are added.
        public string questionText;
        public AnswerData[] answers;

    }
}
