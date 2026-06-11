using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;
using InSite.Domain.Organizations.PerformanceReport;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAssessmentSettings : Command, IHasRun
    {
        public AssessmentSettings Assessments { get; set; }

        public ModifyOrganizationAssessmentSettings(Guid organizationId, AssessmentSettings assessments)
        {
            AggregateIdentifier = organizationId;
            Assessments = assessments;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var isAssessmentsSame = state.Toolkits.Assessments.IsShallowEqual(Assessments);
            var isReportSame = state.Toolkits.Assessments.PerformanceReport.IsShallowEqual(Assessments.PerformanceReport);
            var isReportWeightsSame = ItemWeight.IsEqual(state.Toolkits.Assessments.PerformanceReport.AssessmentTypeWeights, Assessments.PerformanceReport.AssessmentTypeWeights);
            var isReportVariantsSame = ReportVariant.IsEqual(state.Toolkits.Assessments.PerformanceReport.Reports, Assessments.PerformanceReport.Reports);

            if (isAssessmentsSame && isReportSame && isReportWeightsSame && isReportVariantsSame)
                return true;

            if (isReportWeightsSame)
                Assessments.PerformanceReport.AssessmentTypeWeights = null;

            if (isReportVariantsSame)
                Assessments.PerformanceReport.Reports = null;

            if (isReportSame && isReportWeightsSame && isReportVariantsSame)
                Assessments.PerformanceReport = null;

            aggregate.Apply(new OrganizationAssessmentSettingsModified(Assessments));

            return true;
        }
    }
}
