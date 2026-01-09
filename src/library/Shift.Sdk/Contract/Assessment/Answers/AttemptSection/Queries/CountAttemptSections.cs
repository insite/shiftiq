using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttemptSections : Query<int>, IAttemptSectionCriteria
    {
        public Guid? SectionIdentifier { get; set; }

        public bool? IsBreakTimer { get; set; }
        public bool? ShowWarningNextTab { get; set; }

        public string TimerType { get; set; }

        public int? SectionDuration { get; set; }
        public int? TimeLimit { get; set; }

        public DateTimeOffset? SectionCompleted { get; set; }
        public DateTimeOffset? SectionStarted { get; set; }
    }
}