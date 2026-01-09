using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptSectionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? SectionIdentifier { get; set; }

        bool? IsBreakTimer { get; set; }
        bool? ShowWarningNextTab { get; set; }

        string TimerType { get; set; }

        int? SectionDuration { get; set; }
        int? TimeLimit { get; set; }

        DateTimeOffset? SectionCompleted { get; set; }
        DateTimeOffset? SectionStarted { get; set; }
    }
}