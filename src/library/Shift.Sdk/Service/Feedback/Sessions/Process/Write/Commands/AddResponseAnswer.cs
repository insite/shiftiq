using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class AddResponseAnswer : Command
    {
        public AddResponseAnswer(Guid session, Guid question)
        {
            AggregateIdentifier = session;
            Question = question;
        }

        public Guid Question { get; set; }
    }
}