using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseEnrollment : Command, IHasRun
    {
        public Guid CourseEnrollmentId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseEnrollmentMessageType Message { get; set; }

        public int SentCount { get; set; }

        public ModifyCourseEnrollment(Guid courseId, Guid courseEnrollmentId, CourseEnrollmentMessageType message, int sentCount)
        {
            AggregateIdentifier = courseId;
            CourseEnrollmentId = courseEnrollmentId;
            Message = message;
            SentCount = sentCount;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var enrollment = course.Data.Enrollments.Find(x => x.Identifier == CourseEnrollmentId);
            if (enrollment == null)
                return false;

            switch (Message)
            {
                case CourseEnrollmentMessageType.Stalled:
                    if (enrollment.MessageStalledSentCount == SentCount)
                        return false;
                    break;
                case CourseEnrollmentMessageType.Completed:
                    if (enrollment.MessageCompletedSentCount == SentCount)
                        return false;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: ${Message}");
            }

            course.Apply(new CourseEnrollmentModified(CourseEnrollmentId, Message, SentCount));
            return true;
        }
    }
}
