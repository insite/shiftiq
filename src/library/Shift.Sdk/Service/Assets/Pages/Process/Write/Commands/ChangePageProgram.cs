using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageProgram : Command
    {
        public Guid? Program { get; set; }
        public ChangePageProgram(Guid page, Guid? program)
        {
            AggregateIdentifier = page;
            Program = program;
        }
    }
}
