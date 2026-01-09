using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Courses.Write
{
    public class RunCommands : Command
    {
        public ICommand[] Commands { get; set; }

        public RunCommands(Guid courseId, ICommand[] commands)
        {
            AggregateIdentifier = courseId;
            Commands = commands;
        }
    }
}
