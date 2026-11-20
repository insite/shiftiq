using System;

namespace Shift.Common.Integration.Scoop
{
    /// <summary>
    /// Whether a Scoop progress callback reports a finished lesson.
    /// </summary>
    /// <remarks>
    /// Scoop sends the element names the content wrote: cmi.core.lesson_status
    /// for SCORM 1.2, cmi.completion_status and cmi.success_status for SCORM
    /// 2004, a verb and result for xAPI. It also sends the answer projected onto
    /// one field, status, which is read first. The SCORM fields are the fallback
    /// for a callback queued before Scoop sent status, and the 2004 rule mirrors
    /// Scoop's own projection so both sides agree on what counts as done.
    ///
    /// Reading only cmi.core.lesson_status is what let every SCORM 2004 and xAPI
    /// completion arrive, return 200 and change nothing.
    /// </remarks>
    public static class ScoopCallbackStatus
    {
        public static bool IsCompletion(string status, string lessonStatus, string completionStatus, string successStatus)
        {
            var hasStatus = !string.IsNullOrWhiteSpace(status);
            if (hasStatus)
                return IsDone(status);

            var hasLessonStatus = !string.IsNullOrWhiteSpace(lessonStatus);
            if (hasLessonStatus)
                return IsDone(lessonStatus);

            // A failure outranks everything, and a pass does not override an
            // explicit incomplete: passing the quiz is not finishing the course.
            var isFailed = Is(successStatus, "failed");
            if (isFailed)
                return false;

            var isPassed = Is(successStatus, "passed");
            var isIncomplete = Is(completionStatus, "incomplete");
            if (isPassed && !isIncomplete)
                return true;

            return Is(completionStatus, "completed");
        }

        private static bool IsDone(string value)
        {
            return Is(value, "completed") || Is(value, "passed");
        }

        private static bool Is(string value, string expected)
        {
            return string.Equals(value?.Trim(), expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
