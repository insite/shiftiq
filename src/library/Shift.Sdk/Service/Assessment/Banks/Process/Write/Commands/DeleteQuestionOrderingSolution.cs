using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionOrderingSolution : Command
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }

        public DeleteQuestionOrderingSolution(Guid bank, Guid question, Guid solution)
        {
            AggregateIdentifier = bank;
            Question = question;
            Solution = solution;
        }
    }
}
