using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class StartAssessmentQualification : Command
    {
        public StartAssessmentQualification(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}
