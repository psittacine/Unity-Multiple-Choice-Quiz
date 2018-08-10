using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.QuizGame
{
    [System.Serializable]
    public class AnswerData
    {
        public string Question;
        public List<string> AnswerPool;
        public List<bool> IsCorrect;
    }
}
