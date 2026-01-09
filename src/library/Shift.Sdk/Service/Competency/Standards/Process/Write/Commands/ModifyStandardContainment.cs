using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardContainment : Command
    {
        public StandardContainment[] Containments { get; set; }

        public ModifyStandardContainment(Guid parentStandardId, Guid childStandardId, int childSequence, decimal? creditHours, string creditType)
            : this(parentStandardId, new[] { new StandardContainment(childStandardId, childSequence, creditHours, creditType) })
        {
        }

        public ModifyStandardContainment(Guid parentStandardId, StandardContainment[] containments)
        {
            AggregateIdentifier = parentStandardId;
            Containments = containments;
        }
    }
}
