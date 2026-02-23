using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionClassificationChanged : Change
    {
        public Guid Question { get; set; }
        public QuestionClassification Classification { get; set; }

        public QuestionClassificationChanged(Guid question, QuestionClassification classification)
        {
            Question = question;
            Classification = classification;
        }
    }
}
