using System;

using InSite.Domain.Courses;
using InSite.Persistence;

namespace InSite.Web.Change
{
    public class CourseChangeDescriptor : BaseChangeDescriptor
    {
        readonly static CustomField[] _customFields = new CustomField[]
        {
            new CustomField(null, nameof(CourseActivityAdded.ActivityId), (string changeType, object input) =>
            {
                var activity = changeType != typeof(CourseActivityAdded).Name ? ServiceLocator.CourseSearch.GetActivity(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input.ToString(), activity?.ActivityName);
            }),

            new CustomField(null, nameof(CourseModuleAdded.ModuleId), (string changeType, object input) =>
            {
                var module = changeType != typeof(CourseModuleAdded).Name && changeType != typeof(CourseModuleRenamed).Name
                    ? ServiceLocator.CourseSearch.GetModule(Guid.Parse(input.ToString()))
                    : null;
                return new CustomFieldValue(input.ToString(), module?.ModuleName);
            }),

            new CustomField(null, nameof(CourseActivityMoved.MoveToModuleId), (string _, object input) =>
            {
                var module = ServiceLocator.CourseSearch.GetModule(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input.ToString(), module?.ModuleName);
            }),

            new CustomField(null, nameof(CourseUnitAdded.UnitId), (string changeType, object input) =>
            {
                var unit = changeType != typeof(CourseUnitAdded).Name && changeType != typeof(CourseUnitRenamed).Name
                    ? ServiceLocator.CourseSearch.GetUnit(Guid.Parse(input.ToString()))
                    : null;
                return new CustomFieldValue(input.ToString(), unit?.UnitName);
            }),

            new CustomField(null, nameof(CourseModuleMoved.MoveToUnitId), (string changeType, object input) =>
            {
                var unit = ServiceLocator.CourseSearch.GetUnit(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input.ToString(), unit?.UnitName);
            }),

            new CustomField(null, nameof(CourseActivityAssessmentFormConnected.AssessmentFormId), (string _, object input) =>
            {
                var form = input != null ? ServiceLocator.BankSearch.GetForm(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input?.ToString(), form?.FormName);
            }),

            new CustomField(null, nameof(CourseActivityGradeItemConnected.GradeItemId), (string _, object input) =>
            {
                var gradeItem = input != null ? ServiceLocator.RecordSearch.GetGradeItem(Guid.Parse(input.ToString())) : null;
                if (gradeItem == null)
                    return new CustomFieldValue(input?.ToString(), null);

                var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradeItem.GradebookIdentifier);

                return new CustomFieldValue(new (string, string)[]
                {
                    ("GradebookId", gradeItem.GradebookIdentifier.ToString()),
                    ("GradeItemId", gradeItem.GradeItemIdentifier.ToString()),
                    ("Gradebook", gradebook?.GradebookTitle),
                    ("GradeItem", gradeItem.GradeItemName),
                });
            }),

            new CustomField(null, nameof(CourseGradebookConnected.GradebookId), (string _, object input) =>
            {
                var gradebook = input != null ? ServiceLocator.RecordSearch.GetGradebook(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input?.ToString(), gradebook?.GradebookTitle);
            }),

            new CustomField(null, nameof(CourseActivityQuizConnected.QuizId), (string _, object input) =>
            {
                var quiz = input != null ? ServiceLocator.QuizSearch.Select(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input?.ToString(), quiz?.QuizName);
            }),

            new CustomField(null, nameof(CourseActivitySurveyFormConnected.SurveyFormId), (string _, object input) =>
            {
                var form = input != null ? ServiceLocator.SurveySearch.GetSurveyForm(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input?.ToString(), form?.SurveyFormName);
            }),

            new CustomField(null, nameof(CourseTimestampsModified.CreatedBy), (string _, object input) =>
            {
                var user = ServiceLocator.UserSearch.GetUser(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input?.ToString(), user?.FullName);
            }),

            new CustomField(null, nameof(CourseTimestampsModified.ModifiedBy), (string _, object input) =>
            {
                var user = ServiceLocator.UserSearch.GetUser(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input?.ToString(), user?.FullName);
            }),

            new CustomField(null, nameof(CourseCatalogConnected.CatalogId), (string _, object input) =>
            {
                var catalog = CourseSearch.SelectCatalog(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input?.ToString(), catalog?.CatalogName);
            }),

            new CustomField(null, nameof(CourseEnrollmentAdded.LearnerUserId), (string _, object input) =>
            {
                var user = ServiceLocator.UserSearch.GetUser(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input?.ToString(), user?.FullName);
            }),

            new CustomField(null, nameof(CourseFrameworkConnected.FrameworkStandardId), (string _, object input) =>
            {
                var standard = ServiceLocator.StandardSearch.GetStandard(Guid.Parse(input.ToString()));
                return new CustomFieldValue(input?.ToString(), standard?.ContentName);
            }),

            new CustomField(null, nameof(CourseMessageConnected.MessageId), (string _, object input) =>
            {
                var message = input != null ? ServiceLocator.MessageSearch.GetMessage(Guid.Parse(input.ToString())) : null;
                return new CustomFieldValue(input?.ToString(), message?.MessageName);
            }),
        };

        public CourseChangeDescriptor(string[] excludedProperties)
            : base (excludedProperties, _customFields)
        {

        }
    }
}