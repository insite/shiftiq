using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestion : Command
    {
        public Guid Question { get; set; }
        public bool RemoveAllQuestions { get; set; }

        public DeleteQuestion(Guid bank, Guid question, bool removeAllQuestions)
        {
            AggregateIdentifier = bank;
            Question = question;
            RemoveAllQuestions = removeAllQuestions;
        }
    }
}