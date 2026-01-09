using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.CourseObjects
{
    public class CourseEnrollmentRestarted : Change
    {
        public Guid Learner { get; set; }
        public Guid Course { get; set; }

        public CourseEnrollmentRestarted(Guid learner, Guid course)
        {
            Learner = learner;
            Course = course;
        }
    }
}