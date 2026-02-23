using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionLikertColumn : Command
    {
        public Guid Question { get; set; }
        public Guid Column { get; set; }

        public DeleteQuestionLikertColumn(Guid bank, Guid question, Guid column)
        {
            AggregateIdentifier = bank;
            Question = question;
            Column = column;
        }
    }
}
