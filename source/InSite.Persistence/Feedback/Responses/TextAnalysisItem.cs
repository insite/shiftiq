using System;

namespace InSite.Persistence
{
    [Serializable]
    public class TextAnalysisItem
    {
        public int SubmissionId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}
