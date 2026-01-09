using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchAttemptSections : Query<IEnumerable<AttemptSectionMatch>>, IAttemptSectionCriteria
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