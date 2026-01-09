using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ConfigureIntegration : Command
    {
        public bool WithholdGrades { get; set; }
        public bool WithholdDistribution { get; set; }

        public ConfigureIntegration(Guid aggregate, bool withholdGrades, bool withholdDistribution)
        {
            AggregateIdentifier = aggregate;

            WithholdGrades = withholdGrades;
            WithholdDistribution = withholdDistribution;
        }
    }
}
