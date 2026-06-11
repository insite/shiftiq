using System;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class ChangeContent : Command
    {
        public ChangeContent(Guid message, MultilingualString text)
        {
            AggregateIdentifier = message;
            ContentText = text;
        }

        public MultilingualString ContentText { get; set; }
    }
}