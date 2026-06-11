using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityUrl : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }

        public ModifyCourseActivityUrl(Guid courseId, Guid activityId, string url, string target, string type)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            Url = url;
            Target = target;
            Type = type;
        }


        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null ||
                StringHelper.EqualsCaseSensitive(activity.GetTextValue(ActivityField.ActivityUrl), Url, true)
                && StringHelper.EqualsCaseSensitive(activity.GetTextValue(ActivityField.ActivityUrlTarget), Target, true)
                && StringHelper.EqualsCaseSensitive(activity.GetTextValue(ActivityField.ActivityUrlType), Type, true)
                )
            {
                return false;
            }

            course.Apply(new CourseActivityUrlModified(ActivityId, Url, Target, Type));
            return true;
        }
    }
}
