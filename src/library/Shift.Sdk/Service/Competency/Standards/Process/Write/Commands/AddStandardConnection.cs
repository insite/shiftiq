using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class AddStandardConnection : Command
    {
        public StandardConnection[] Connections { get; set; }

        public AddStandardConnection(Guid fromStandardId, Guid toStandardId, string connectionType)
            : this(fromStandardId, new[] { new StandardConnection(toStandardId, connectionType) })
        {
        }

        public AddStandardConnection(Guid fromStandardId, StandardConnection[] connections)
        {
            AggregateIdentifier = fromStandardId;
            Connections = connections;
        }
    }
}
