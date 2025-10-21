using System;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    public class SummaryCounterData
    {
        public Counter[] ProgramNames { get; set; }
        public Counter[] GradebookNames { get; set; }
        public Counter[] EnrollmentStatuses { get; set; }
        public Counter[] EngagementStatuses { get; set; }
        public Counter[] LearnerGenders { get; set; }
        public Counter[] LearnerReferrers { get; set; }
        public Counter[] ImmigrationStatuses { get; set; }
        public Counter[] ImmigrationDestinations { get; set; }
        public Counter[] LearnerCitizenships { get; set; }
    }

    public class SummaryExportDataGroup
    {
        public string Heading { get; set; }
        public Counter[] Counters { get; set; }
    }

    public class SummaryCounterDataEventArgs : EventArgs
    {
        public SummaryCounterData SummaryCounterData { get; }
        public VLearnerActivityFilter Filter { get; }

        public SummaryCounterDataEventArgs(SummaryCounterData summaryCounterData, VLearnerActivityFilter filter)
        {
            SummaryCounterData = summaryCounterData;
            Filter = filter;
        }
    }
}