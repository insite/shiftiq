using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ImportSet : Command
    {
        public Set Set { get; set; }

        public ImportSet(Guid bank, Set set)
        {
            AggregateIdentifier = bank;
            Set = set;
        }
    }
}