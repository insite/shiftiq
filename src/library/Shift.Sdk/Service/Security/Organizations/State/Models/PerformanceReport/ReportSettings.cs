using System;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations.PerformanceReport
{
    [Serializable]
    public class ReportSettings
    {
        public bool Enabled { get; set; }
        public ItemWeight[] AssessmentTypeWeights { get; set; }
        public ReportVariant[] Reports { get; set; }

        public ReportSettings()
        {
            AssessmentTypeWeights = new ItemWeight[0];
            Reports = new ReportVariant[0];
        }

        public bool ShouldSerializeAssessmentTypeWeights() => AssessmentTypeWeights.IsNotEmpty();
        public bool ShouldSerializeReports() => Reports.IsNotEmpty();

        public bool IsShallowEqual(ReportSettings other)
        {
            return Enabled == other.Enabled;
        }

        public ReportSettings Clone()
        {
            return new ReportSettings
            {
                Enabled = Enabled,
                AssessmentTypeWeights = AssessmentTypeWeights.Select(x => x.Clone()).ToArray(),
                Reports = Reports.Select(x => x.Clone()).ToArray()
            };
        }
    }
}
