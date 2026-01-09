using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QProgress
    {
        public Guid ProgressIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ProgressComment { get; set; }
        public string ProgressPassOrFail { get; set; }
        public string ProgressStatus { get; set; }
        public string ProgressText { get; set; }

        public bool ProgressIsCompleted { get; set; }
        public bool ProgressIsDisabled { get; set; }
        public bool ProgressIsLocked { get; set; }
        public bool ProgressIsPublished { get; set; }
        public bool ProgressIsIgnored { get; set; }

        public int? ProgressElapsedSeconds { get; set; }
        public int? ProgressRestartCount { get; set; }

        public decimal? ProgressMaxPoints { get; set; }
        public decimal? ProgressNumber { get; set; }
        public decimal? ProgressPercent { get; set; }
        public decimal? ProgressPoints { get; set; }

        public bool NoScore =>
            ProgressNumber == null
            && ProgressPercent == null
            && ProgressPoints == null
            && string.IsNullOrEmpty(ProgressText);

        public DateTimeOffset? ProgressCompleted { get; set; }
        public DateTimeOffset? ProgressGraded { get; set; }
        public DateTimeOffset? ProgressStarted { get; set; }
        public DateTimeOffset ProgressAdded { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual QGradeItem GradeItem { get; set; }
        public virtual VUser Learner { get; set; }
    }
}
