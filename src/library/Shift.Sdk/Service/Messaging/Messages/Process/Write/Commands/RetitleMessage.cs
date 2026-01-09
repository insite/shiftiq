using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Messages.Write
{
    public class RetitleMessage : Command
    {
        public RetitleMessage(Guid message, MultilingualString title)
        {
            AggregateIdentifier = message;
            
            Title = title;
        }

        public MultilingualString Title { get; set; }
    }
}