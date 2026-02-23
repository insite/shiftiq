using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class SelectResponseOption : Command
    {
        public SelectResponseOption(Guid session, Guid item)
        {
            AggregateIdentifier = session;
            Item = item;
        }

        public Guid Item { get; set; }
    }
}