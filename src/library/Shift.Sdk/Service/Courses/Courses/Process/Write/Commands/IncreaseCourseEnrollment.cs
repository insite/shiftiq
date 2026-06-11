using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class IncreaseCourseEnrollment : Command, IHasRun
    {
        public Guid CourseEnrollmentId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseEnrollmentMessageType Message { get; set; }

        public IncreaseCourseEnrollment(Guid courseId, Guid courseEnrollmentId, CourseEnrollmentMessageType message)
        {
            AggregateIdentifier = courseId;
            CourseEnrollmentId = courseEnrollmentId;
            Message = message;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var enrollment = course.Data.Enrollments.Find(x => x.Identifier == CourseEnrollmentId);
            if (enrollment == null)
                return false;

            course.Apply(new CourseEnrollmentIncreased(CourseEnrollmentId, Message));
            return true;
        }
    }
}
