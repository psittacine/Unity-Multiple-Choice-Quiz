namespace Assets.Scripts.QuizGame
{
    [System.Serializable]
    public static class RoundData
    {
        // This is where we keep all the settings that will operate across the scenes.
        public static int TimeLimitInSeconds;
        public static int PointsAddedForCorrectAnswer = 10;
        public static string CategoryParameters;
        public static bool RandomQuestions;

    }
}
