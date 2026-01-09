using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Organizations.Write
{
    public class RunCommands : Command
    {
        public ICommand[] Commands { get; set; }

        public RunCommands(Guid organizationId, ICommand[] commands)
        {
            AggregateIdentifier = organizationId;
            Commands = commands;
        }
    }
}
