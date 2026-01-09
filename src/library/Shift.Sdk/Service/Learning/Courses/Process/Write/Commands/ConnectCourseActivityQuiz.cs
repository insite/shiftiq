using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseActivityQuiz : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid? QuizId { get; set; }

        public ConnectCourseActivityQuiz(Guid courseId, Guid activityId, Guid? quizId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            QuizId = quizId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity.GetGuidValue(ActivityField.QuizIdentifier) == QuizId)
                return false;

            course.Apply(new CourseActivityQuizConnected(ActivityId, QuizId));
            return true;
        }
    }
}
