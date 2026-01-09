using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class RenameMessage : Command
    {
        public RenameMessage(Guid message, string name)
        {
            AggregateIdentifier = message;
            Name = name;
        }

        public string Name { get; set; }
    }
}