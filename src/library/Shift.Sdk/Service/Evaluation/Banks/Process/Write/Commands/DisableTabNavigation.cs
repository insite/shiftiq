using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DisableTabNavigation : Command
    {
        public Guid Specification { get; set; }

        public DisableTabNavigation(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
