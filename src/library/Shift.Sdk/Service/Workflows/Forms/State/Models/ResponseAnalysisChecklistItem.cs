using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisChecklistItem
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid OptionIdentifier { get; set; }
        public decimal OptionPoints { get; set; }
    }
}
