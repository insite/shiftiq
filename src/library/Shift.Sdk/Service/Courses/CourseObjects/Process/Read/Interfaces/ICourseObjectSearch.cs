using System;

namespace InSite.Application.Courses.Read
{
    public class ActivityConnection
    {
        public string Type { get; set; }
        public string Condition { get; set; }

        public Guid FromIdentifier { get; set; }
        public Guid ToIdentifier { get; set; }
    }

    public interface ICourseObjectSearch
    {
        QActivity FindActivityByAssessmentForm(Guid form);
        QActivity FindActivityBySurveyForm(Guid form);
        QActivity FindActivityByGradeItem(Guid gradeItem);
        QCourse FindCourseByGradebook(Guid gradebook);
        bool IsActivityCompleted(Guid activityId, Guid userId);
        QCourseEnrollment GetCourseEnrollment(Guid course, Guid user);
        QCourse GetCourse(Guid course);
        Guid? GetActivityGradebookIdBySurveyForm(Guid form);

        string GetOrganizationCode(Guid organization);
    }
}
