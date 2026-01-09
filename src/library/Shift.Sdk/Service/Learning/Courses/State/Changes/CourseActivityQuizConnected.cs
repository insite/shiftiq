using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityQuizConnected : Change
    {
        public Guid ActivityId { get; set; }
        public Guid? QuizId { get; set; }

        public CourseActivityQuizConnected(Guid activityId, Guid? quizId)
        {
            ActivityId = activityId;
            QuizId = quizId;
        }
    }
}
