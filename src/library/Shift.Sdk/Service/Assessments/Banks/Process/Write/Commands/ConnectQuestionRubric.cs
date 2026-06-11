using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ConnectQuestionRubric : Command
    {
        public Guid Question { get; set; }
        public Guid Rubric { get; set; }

        public ConnectQuestionRubric(Guid bank, Guid question, Guid rubric)
        {
            AggregateIdentifier = bank;
            Question = question;
            Rubric = rubric;
        }
    }
}
