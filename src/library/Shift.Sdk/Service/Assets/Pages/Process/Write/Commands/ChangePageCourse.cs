using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageCourse : Command
    {
        public Guid? Course { get; set; }
        public ChangePageCourse(Guid page, Guid? course)
        {
            AggregateIdentifier = page;
            Course = course;
        }
    }
}
