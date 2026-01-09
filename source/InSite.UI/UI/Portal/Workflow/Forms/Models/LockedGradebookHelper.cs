using System;

namespace InSite.UI.Portal.Workflow.Forms.Models
{
    static class LockedGradebookHelper
    {
        public static bool HasLockedGradebook(Guid surveyId, string hook)
        {
            return IsGradebookLockedByHook(hook) || IsGradebookLockedByForm(surveyId);
        }

        private static bool IsGradebookLockedByHook(string hook)
        {
            var gradeItemId = !string.IsNullOrEmpty(hook)
                ? ServiceLocator.RecordSearch.GetGradeItemByHook(hook)?.GradeItemIdentifier
                : null;

            if (gradeItemId == null)
                return false;

            var gradeItem = ServiceLocator.RecordSearch.GetGradeItem(gradeItemId.Value);
            var gradebook = gradeItem != null ? ServiceLocator.RecordSearch.GetGradebook(gradeItem.GradebookIdentifier) : null;

            return gradebook != null && gradebook.IsLocked;
        }

        private static bool IsGradebookLockedByForm(Guid surveyId)
        {
            var gradebookId = ServiceLocator.CourseObjectSearch.GetActivityGradebookIdBySurveyForm(surveyId);
            if (gradebookId == null)
                return false;

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradebookId.Value);

            return gradebook != null && gradebook.IsLocked;
        }
    }
}