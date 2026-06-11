using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class RenameGradebook : Command
    {
        public string Name { get; set; }

        public RenameGradebook(Guid record, string name)
        {
            AggregateIdentifier = record;
            Name = name;
        }
    }
}
