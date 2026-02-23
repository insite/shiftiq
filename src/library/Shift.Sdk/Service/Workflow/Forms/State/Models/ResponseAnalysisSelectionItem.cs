using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisSelectionItem
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid OptionIdentifier { get; set; }
        public decimal? OptionPoints { get; set; }
        public int AnswerCount { get; set; }
    }
}
