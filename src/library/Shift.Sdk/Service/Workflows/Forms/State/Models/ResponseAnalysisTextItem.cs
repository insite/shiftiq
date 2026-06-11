using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisTextItem
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public string AnswerText { get; set; }

    }
}
