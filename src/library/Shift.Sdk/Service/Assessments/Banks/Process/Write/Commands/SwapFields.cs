using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class SwapFields : Command
    {
        public Guid A { get; set; }
        public Guid B { get; set; }

        public SwapFields(Guid bank, Guid a, Guid b)
        {
            AggregateIdentifier = bank;

            A = a;
            B = b;
        }
    }
}