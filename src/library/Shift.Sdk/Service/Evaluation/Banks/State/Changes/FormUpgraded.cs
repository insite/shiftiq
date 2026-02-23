using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FormUpgraded : Change
    {
        public Guid Source { get; set; }
        public Guid Destination { get; set; }
        public string NewName { get; set; }

        public FormUpgraded(Guid source, Guid destination, string newName)
        {
            Source = source;
            Destination = destination;
            NewName = newName;
        }
    }
}
