using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisCommentItem
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public string AnswerComment { get; set; }
    }
}
