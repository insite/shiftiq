using System;
using System.Collections.Generic;

namespace InSite.Application.Courses.Read
{
    public interface ICourseSearch
    {
        QCourse GetCourse(Guid courseId);
        QUnit GetUnit(Guid unitId);
        QModule GetModule(Guid moduleId);
        QActivity GetActivity(Guid activityId);
        QActivity GetActivityBySurveyForm(Guid formId);
        QActivity GetActivityByGradeItem(Guid gradeItemId);
        List<QCourseEnrollment> GetEnrollments(Guid courseId);
        List<QCoursePrerequisite> GetPrerequisites(Guid courseId);
        List<QActivityCompetency> GetActivityCompetencies(Guid activityId);

        Guid? GetCourseIdByActivityId(Guid activityId);
        List<QCoursePrerequisite> GetPrerequisitesByTrigger(Guid triggerId);
    }
}
