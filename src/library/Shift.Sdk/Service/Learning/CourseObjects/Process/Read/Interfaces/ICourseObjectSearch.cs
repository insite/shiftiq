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
        TActivity FindActivityByAssessmentForm(Guid form);
        TActivity FindActivityBySurveyForm(Guid form);
        TActivity FindActivityByGradeItem(Guid gradeItem);
        TCourse FindCourseByGradebook(Guid gradebook);
        bool IsActivityCompleted(Guid activityId, Guid userId);
        TCourseUser GetCourseUser(Guid course, Guid user);
        TCourse GetCourse(Guid course);
        LegacyPrerequisite[] GetLegacyPrerequisites();
        Guid? GetActivityGradebookIdBySurveyForm(Guid form);

        string GetOrganizationCode(Guid organization);
    }

    public class LegacyPrerequisite
    {
        public string PrerequisiteCondition { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid ActivityIdentifier { get; set; }
        public Guid? TriggerActivityIdentifier { get; set; }
    }
}
