using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionClassification : Command
    {
        public Guid Question { get; set; }
        public QuestionClassification Classification { get; set; }

        public ChangeQuestionClassification(Guid bank, Guid question, QuestionClassification classification)
        {
            AggregateIdentifier = bank;
            Question = question;
            Classification = classification;
        }
    }
}
