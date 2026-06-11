using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageSequence : Command
    {
        public int Sequence { get; set; }
        public ChangePageSequence(Guid page, int sequence)
        {
            AggregateIdentifier = page;
            Sequence = sequence;
        }
    }
}
