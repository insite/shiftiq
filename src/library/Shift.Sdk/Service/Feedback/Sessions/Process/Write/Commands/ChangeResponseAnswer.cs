using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class ChangeResponseAnswer : Command
    {
        public ChangeResponseAnswer(Guid session, Guid question, string answer)
        {
            AggregateIdentifier = session;
            Question = question;
            Answer = answer;
        }

        public Guid Question { get; set; }
        public string Answer { get; set; }
    }
}