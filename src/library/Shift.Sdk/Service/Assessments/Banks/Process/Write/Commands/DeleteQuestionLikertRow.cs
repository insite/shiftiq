using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionLikertRow : Command
    {
        public Guid Question { get; set; }
        public Guid Row { get; set; }

        public DeleteQuestionLikertRow(Guid bank, Guid question, Guid row)
        {
            AggregateIdentifier = bank;
            Question = question;
            Row = row;
        }
    }
}
