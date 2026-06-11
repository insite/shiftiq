using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UpgradeForm : Command
    {
        public Guid Source { get; set; }
        public Guid Destination { get; set; }
        public string NewName { get; set; }

        public UpgradeForm(Guid bank, Guid source, Guid destination, string newName)
        {
            AggregateIdentifier = bank;
            Source = source;
            Destination = destination;
            NewName = newName;
        }
    }
}