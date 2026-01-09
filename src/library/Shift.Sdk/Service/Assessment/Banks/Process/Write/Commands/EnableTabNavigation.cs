using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class EnableTabNavigation : Command
    {
        public Guid Specification { get; set; }

        public EnableTabNavigation(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
