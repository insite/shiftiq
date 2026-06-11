using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupLifetime : Command
    {
        public int? Quantity { get; }
        public string Unit { get; }

        public ChangeGroupLifetime(Guid group, int? quantity, string unit)
        {
            AggregateIdentifier = group;
            Quantity = quantity;
            Unit = unit;
        }
    }
}