using System;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptSection
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid? SectionIdentifier { get; set; }
        public int SectionIndex { get; set; }
        public int? TimeLimit { get; set; }
        public string TimerType { get; set; }
        public bool IsBreakTimer { get; set; }
        public bool ShowWarningNextTab { get; set; }
        public DateTimeOffset? SectionStarted { get; set; }
        public DateTimeOffset? SectionCompleted { get; set; }
        public int? SectionDuration { get; set; }

        public virtual QAttempt Attempt { get; set; }
    }
}
