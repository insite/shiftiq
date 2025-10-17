using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisCategoryItem
    {
        public Guid QuestionIdentifier { get; set; }
        public string OptionCategory { get; set; }
        public int AnswerCount { get; set; }
    }
}
