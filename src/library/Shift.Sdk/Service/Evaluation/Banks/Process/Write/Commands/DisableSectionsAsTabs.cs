using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DisableSectionsAsTabs : Command
    {
        public Guid Specification { get; set; }

        public DisableSectionsAsTabs(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
