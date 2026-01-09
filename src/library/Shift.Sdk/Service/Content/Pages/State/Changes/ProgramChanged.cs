using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class ProgramChanged : Change
    {
        public Guid? Program { get; set; }
        public ProgramChanged(Guid? program)
        {
            Program = program;
        }
    }
}
