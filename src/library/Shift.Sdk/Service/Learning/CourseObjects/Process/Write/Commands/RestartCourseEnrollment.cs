using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Courses.Write
{
    public class RestartCourseEnrollment : Command
    {
        public Guid Learner { get; set; }
        public Guid Course { get; set; }

        public RestartCourseEnrollment(Guid learner, Guid course)
        {
            Learner = learner;
            Course = course;
        }
    }
}
