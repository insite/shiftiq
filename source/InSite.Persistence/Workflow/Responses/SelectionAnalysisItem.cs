using System;

namespace InSite.Persistence
{
    [Serializable]
    public class SelectionAnalysisItem
    {
        public int QuestionId { get; private set; }
        public int? OptionId { get; private set; }
        public bool AnswerCheck { get; private set; }
        public int AnswerFrequency { get; set; }
        public int AnswerScore { get; private set; }

        public SelectionAnalysisItem(int questionId, int optionId, int answerFrequencey, int answerScore)
        {
            QuestionId = questionId;
            OptionId = optionId;
            AnswerFrequency = answerFrequencey;
            AnswerScore = answerScore;
        }

        public SelectionAnalysisItem(int questionId, bool answerCheck, int answerFrequencey, int answerScore)
        {
            QuestionId = questionId;
            AnswerCheck = answerCheck;
            AnswerFrequency = answerFrequencey;
            AnswerScore = answerScore;
        }
    }
}
