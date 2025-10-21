using System;

namespace InSite.Persistence
{
    [Serializable]
    public class CategoryAnalysisItem
    {
        public int QuestionId { get; private set; }
        public string Category { get; private set; }
        public int AnswerFrequency { get; set; }

        public CategoryAnalysisItem(int questionId, string category, int answerFrequencey)
        {
            QuestionId = questionId;
            Category = category;
            AnswerFrequency = answerFrequencey;
        }
    }
}
