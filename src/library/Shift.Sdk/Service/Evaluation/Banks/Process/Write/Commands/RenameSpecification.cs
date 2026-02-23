using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class RenameSpecification : Command
    {
        public Guid Specification { get; set; }
        public string Name { get; set; }
        
        public RenameSpecification(Guid bank, Guid spec, string name)
        {
            AggregateIdentifier = bank;
            Specification = spec;
            Name = name;
        }
    }
}