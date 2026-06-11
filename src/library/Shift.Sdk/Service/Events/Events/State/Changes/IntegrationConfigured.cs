
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class IntegrationConfigured : Change
    {
        public bool WithholdGrades { get; set; }
        public bool WithholdDistribution { get; set; }

        public IntegrationConfigured(bool withholdGrades, bool withholdDistribution)
        {
            WithholdGrades = withholdGrades;
            WithholdDistribution = withholdDistribution;
        }
    }
}
