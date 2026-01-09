using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Gradebooks.Write
{
    public class DuplicateGradebook : Command
    {
        public DuplicateGradebook(
            Guid newRecord,
            Guid tenant,
            Guid source,
            string name,
            GradebookType type,
            Guid? @event,
            Guid? achievement,
            Guid? framework
            )
        {
            AggregateIdentifier = newRecord;
            Tenant = tenant;
            Source = source;
            Name = name;
            Type = type;
            Event = @event;
            Achievement = achievement;
            Framework = framework;
        }

        public Guid Tenant { get; set; }

        /// <summary>
        /// This is the aggregate identifier for the gradebook from which we will copy. 
        /// </summary>
        /// <remarks>
        /// Important Note: This command must be sent to the new gradebook and NOT to the gradebook being duplicated!
        /// </remarks>
        public Guid Source { get; }

        public string Name { get; }
        public GradebookType Type { get; }
        public Guid? Event { get; }
        public Guid? Achievement { get; }
        public Guid? Framework { get; }
    }
}
