using System;

using Shift.Common;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class LearningResult
    {
        public string Grade { get; set; }
        public string Status { get; set; }

        public decimal? Score { get; set; }

        public string Icon
        {
            get
            {
                if (IsCompleted)
                    return "fas fa-check text-success";

                if (IsLocked)
                    return "fas fa-lock-alt text-danger";

                if (Status == "Completed" && (Grade.IsNotEmpty() || Score.HasValue) && IsFailed || Status == "Incomplete")
                    return "fas fa-times text-danger";

                if (IsStarted)
                    return "fas fa-hourglass-start text-info";

                return "fas fa-circle text-primary";
            }
        }

        public DateTimeOffset? Completed { get; set; }

        public bool IsCompletedOrFailed => Completed.HasValue || Status == "Completed" || Status == "Complete";
        public bool IsCompleted => IsCompletedOrFailed && !IsFailed;
        public bool IsLocked => Status == "Locked";
        public bool IsStarted => Status == "Started";
        public bool IsUnlocked => !IsLocked;

        public bool IsPassed => Grade == "Pass";
        public bool IsFailed => Grade == "Fail";

        public bool IsHidden { get; set; }

        public LearningResultNavigation Navigation { get; set; }

        public LearningResult()
        {
            Navigation = new LearningResultNavigation();
        }
    }

    [Serializable]
    public class AttemptResult
    {
        public Guid Form { get; set; }
        public decimal? Score { get; set; }
        public bool IsPass { get; set; }
        public bool IsFail => !IsPass;
    }

    [Serializable]
    public class GradeItemResult
    {
        public Guid GradeItem { get; set; }
        public decimal? Score { get; set; }
        public bool IsPass { get; set; }
        public bool IsFail => Score.HasValue && !IsPass;
    }
}