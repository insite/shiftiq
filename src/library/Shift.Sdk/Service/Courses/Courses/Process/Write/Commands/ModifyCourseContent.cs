using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseContent : Command, IHasRun
    {
        public ContentContainer CourseContent { get; set; }

        public ModifyCourseContent(Guid courseId, ContentContainer courseContent)
        {
            AggregateIdentifier = courseId;
            CourseContent = courseContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.Content.IsEqual(CourseContent))
                return false;

            course.Apply(new CourseContentModified(CourseContent));
            return true;
        }
    }
}
