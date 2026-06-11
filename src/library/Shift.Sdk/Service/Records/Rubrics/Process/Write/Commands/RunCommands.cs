using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Rubrics.Write
{
    public class RunCommands : Command
    {
        public ICommand[] Commands { get; set; }

        public RunCommands(Guid rubricId, ICommand[] commands)
        {
            AggregateIdentifier = rubricId;
            Commands = commands;
        }
    }
}
