using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteSection : Command
    {
        public Guid Section { get; set; }

        public DeleteSection(Guid bank, Guid section)
        {
            AggregateIdentifier = bank;
            Section = section;
        }
    }
}