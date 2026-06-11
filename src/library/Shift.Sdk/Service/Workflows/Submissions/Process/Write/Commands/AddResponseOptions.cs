using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class AddResponseOptions : Command
    {
        public AddResponseOptions(Guid session, Guid question, Guid[] items)
        {
            AggregateIdentifier = session;
            Question = question;
            Items = items;
        }

        public Guid Question { get; set; }
        public Guid[] Items { get; set; }
    }
}