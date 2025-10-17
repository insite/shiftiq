using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Standards.Read;
using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IRecordSearch
    {
        List<AcademicYearOutcome> GetAcademicYearOutcome(Guid organizationIdentifier, IEnumerable<Guid> gradebookPeriods);
        List<OutcomeSummary> GetOutcomeSummary(QGradebookFilter filter);
        List<CourseOutcomeSummary> GetCourseOutcomeSummary(QGradebookFilter filter);
        List<MostImprovedStudent> GetMostImprovedStudents(QProgressFilter filter);
        List<PassingRateSummary> GetPassingRateSummaries(QProgressFilter filter);
        List<TopStudentSummary> GetTopStudentSummaries(QProgressFilter filter);
        List<LowestScoreStudent> GetLowestScoreStudents(QProgressFilter filter);
        List<VStandard> GetGradebookStandards(Guid id);

        int CountEnrollments(QEnrollmentFilter filter);
        List<QEnrollment> GetEnrollments(QEnrollmentFilter filter, params Expression<Func<QEnrollment, object>>[] includes);
        List<EnrollmentForPeriodGrid> GetEnrollmentsForPeriodGrid(QEnrollmentFilter filter);
        bool EnrollmentExists(Guid gradebook);
        bool EnrollmentExists(Guid gradebook, Guid learner);
        QEnrollment GetEnrollment(Guid gradebook, Guid learner);

        GradebookState GetGradebookState(Guid id);

        int CountGradebooks(QGradebookFilter filter);
        bool GradebookExists(Guid gradebook);
        QGradebook GetGradebook(Guid id, params Expression<Func<QGradebook, object>>[] includes);

        QGradebook GetGradebookByReference(string reference, Guid organization, params Expression<Func<QGradebook, object>>[] includes);
        List<QGradebook> GetGradebooks(QGradebookFilter filter, params Expression<Func<QGradebook, object>>[] includes);
        List<QGradebook> GetEventGradebooks(Guid eventId);
        List<QGradebook> GetRecentGradebooks(QGradebookFilter filter, int? take = null);

        int CountGradebookScores(QProgressFilter filter);
        List<QProgress> GetGradebookScores(QProgressFilter filter, params Expression<Func<QProgress, object>>[] includes);
        QProgress GetProgress(Guid gradebook, Guid gradeitem, Guid learner);
        Guid? GetProgressIdentifier(Guid gradebook, Guid gradeitem, Guid learner);
        QProgress GetProgress(Guid progress, params Expression<Func<QProgress, object>>[] includes);
        bool RecordHasProgress(Guid record);
        bool ItemHasProgress(Guid gradebook, Guid gradeItem);
        bool ProgressExists(Guid gradebook, Guid gradeitem, Guid learner);
        T BindProgress<T>(
            Expression<Func<QProgress, T>> binder,
            Expression<Func<QProgress, bool>> filter,
            string modelSort = null,
            string entitySort = null);
        T[] BindProgresses<T>(
            Expression<Func<QProgress, T>> binder,
            Expression<Func<QProgress, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        int CountGradeItems(QGradeItemFilter filter);
        bool GradeItemExists(Guid item);
        bool IsGradeItemCodeUniqe(Guid gradebook, Guid excludeGradeItem, string code);
        QGradeItem GetGradeItem(Guid item);
        QGradeItem GetGradeItemByHook(string hook);
        T[] BindGradeItems<T>(
            Expression<Func<QGradeItem, T>> binder,
            Expression<Func<QGradeItem, bool>> filter,
            string modelSort = null,
            string entitySort = null);


        List<QGradeItem> GetGradeItems(Guid gradebook);
        List<QGradeItem> GetGradeItems(QGradeItemFilter filter, params Expression<Func<QGradeItem, object>>[] includes);

        List<VGradeItemHierarchy> GetGradeItemHierarchies(Guid report);
        string BuildGradebookReport(Guid report);

        QGradebookCompetencyValidation GetValidation(Guid gradebookIdentifier, Guid userIdentifier, Guid competencyIdentifier, params Expression<Func<QGradebookCompetencyValidation, object>>[] includes);
        int CountValidations(QGradebookCompetencyValidationFilter filter);
        List<QGradebookCompetencyValidation> GetValidations(QGradebookCompetencyValidationFilter filter, params Expression<Func<QGradebookCompetencyValidation, object>>[] includes);

        AddEnrollment CreateCommandToAddEnrollment(Guid? enrollment, Guid gradebook, Guid learner, Guid? period, DateTimeOffset? time, string comment);
        AddProgress CreateCommandToAddProgress(Guid? progress, Guid gradebook, Guid gradeitem, Guid user);
        List<AddProgress> CreateCommandsToAddProgresses(Guid? progress, Guid gradebook, Guid user, IEnumerable<Guid> gradeitems);

        int CountStatements(VStatementFilter filter);
        List<VStatement> GetStatements(VStatementFilter filter, params Expression<Func<VStatement, object>>[] includes);
    }
}