using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class VerifyAssessmentQuestionOrder : Command
    {
        public Guid Form { get; set; }
        public Guid[] Questions { get; set; }

        public VerifyAssessmentQuestionOrder(Guid bank, Guid form, Guid[] questions)
        {
            AggregateIdentifier = bank;
            Form = form;
            Questions = questions;
        }
    }
}