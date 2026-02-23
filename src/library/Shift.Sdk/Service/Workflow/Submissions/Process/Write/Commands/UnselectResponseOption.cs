using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class UnselectResponseOption : Command
    {
        public UnselectResponseOption(Guid session, Guid item)
        {
            AggregateIdentifier = session;
            Item = item;
        }

        public Guid Item { get; set; }
    }
}