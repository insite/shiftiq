using System;
using System.Linq;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    static class LockedGradebookHelper
    {
        public static bool HasLockedGradebook(Guid attemptId, Guid formId, string formHook)
        {
            return HasLockedGradebookByAttempt(attemptId, formId)
                || IsCourseLocked(formId)
                || IsFormLockedByHook(formHook);
        }

        public static bool HasLockedGradebook(Guid formId, string formHook)
        {
            return IsCourseLocked(formId)
                || IsFormLockedByHook(formHook)
                || IsFormLockedByItems(formId);
        }

        private static bool HasLockedGradebookByAttempt(Guid attemptId, Guid formId)
        {
            var form = ServiceLocator.BankSearch.GetFormData(formId);
            if (form?.Gradebook == null)
                return false;

            var attemptQuestions = ServiceLocator.AttemptSearch.GetAttemptQuestions(attemptId);
            var attemptQuestionIds = attemptQuestions.Select(x => x.QuestionIdentifier).ToList();

            var hasGradeItemQuestion = form.GetQuestions()
                .Where(x => x.GradeItems.ContainsKey(form.Identifier) && attemptQuestionIds.Contains(x.Identifier))
                .Any();

            if (!hasGradeItemQuestion)
                return false;

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(form.Gradebook.Value);

            return gradebook != null && gradebook.IsLocked;
        }

        private static bool IsCourseLocked(Guid formId)
        {
            var gradeItemId = ServiceLocator.CourseObjectSearch.FindActivityByAssessmentForm(formId)?.GradeItemIdentifier;
            return gradeItemId.HasValue && IsGradebookLocked(gradeItemId.Value);
        }

        private static bool IsFormLockedByHook(string formHook)
        {
            var gradeItemId = !string.IsNullOrEmpty(formHook)
                ? ServiceLocator.RecordSearch.GetGradeItemByHook(formHook)?.GradeItemIdentifier
                : null;

            return gradeItemId.HasValue && IsGradebookLocked(gradeItemId.Value);
        }

        private static bool IsFormLockedByItems(Guid formId)
        {
            var form = ServiceLocator.BankSearch.GetFormData(formId);
            if (form?.Gradebook == null)
                return false;

            var questions = form.GetQuestions();

            if (!questions.Any(x => x.GradeItems.ContainsKey(form.Identifier)))
                return false;

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(form.Gradebook.Value);

            return gradebook != null && gradebook.IsLocked;
        }

        private static bool IsGradebookLocked(Guid gradeItemId)
        {
            var gradeItem = ServiceLocator.RecordSearch.GetGradeItem(gradeItemId);
            var gradebook = gradeItem != null ? ServiceLocator.RecordSearch.GetGradebook(gradeItem.GradebookIdentifier) : null;

            return gradebook != null && gradebook.IsLocked;
        }
    }
}