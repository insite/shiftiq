using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
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