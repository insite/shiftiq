using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Records.Reports.LearnerActivity.Models;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Reports.LearnerActivity.Controls
{
    public partial class SummaryTables : BaseUserControl
    {
        private VLearnerActivityFilter Filter
        {
            get => (VLearnerActivityFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadSummaryTables.Click += DownloadSummaryTables_Click;
        }

        #endregion

        #region Event handlers

        private void DownloadSummaryTables_Click(object sender, EventArgs e)
        {
            var data = GetCurrentSummariesAsXlsx();

            Response.SendFile("learner-activity-summaries", "xlsx", data);
        }

        #endregion

        #region Export

        private byte[] GetCurrentSummariesAsXlsx()
        {
            var list = VLearnerActivitySearch.Bind(x => x, Filter);
            var summaryData = Filter.IsSummaryCountStrategy
                ? GetLearnersSummaryCounterData(list)
                : GetActivitiesSummaryCounterData(list);

            var program = CreateSheet("Program", new[]
            {
                new SummaryExportDataGroup { Heading = "Program Name", Counters = summaryData.ProgramNames },
                new SummaryExportDataGroup { Heading = "Gradebook  Name", Counters = summaryData.GradebookNames },
                new SummaryExportDataGroup { Heading = "Gradebook Enrollment Status", Counters = summaryData.EnrollmentStatuses }
            });
            var engagement = CreateSheet("Engagement", new[]
            {
                new SummaryExportDataGroup { Heading = "Sign In Activity", Counters = summaryData.EngagementStatuses }
            });
            var learner = CreateSheet("Learner", new[]
            {
                new SummaryExportDataGroup { Heading = "Gender", Counters = summaryData.LearnerGenders },
                new SummaryExportDataGroup { Heading = "Referrer Name", Counters = summaryData.LearnerReferrers }
            });
            var immigration = CreateSheet("Immigration", new[]
            {
                new SummaryExportDataGroup { Heading = "Immigration Status", Counters = summaryData.ImmigrationStatuses },
                new SummaryExportDataGroup { Heading = "Immigration Destination", Counters = summaryData.ImmigrationDestinations },
                new SummaryExportDataGroup { Heading = "Citizenship", Counters = summaryData.LearnerCitizenships }
            });

            return XlsxWorksheet.GetBytes(program, engagement, learner, immigration);
        }

        private static XlsxWorksheet CreateSheet(string sheetName, IEnumerable<SummaryExportDataGroup> groups)
        {
            var headerLeftStyle = new XlsxCellStyle { IsBold = true };
            var leftStyle = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };

            var sheet = new XlsxWorksheet(sheetName);
            sheet.Columns[0].Width = 40; // Name
            sheet.Columns[1].Width = 10; // Value

            var row = 0;

            foreach (var group in groups)
            {
                sheet.Cells.Add(new XlsxCell(0, ++row) { Value = group.Heading, Style = headerLeftStyle });

                for (var i = 0; i < group.Counters.Length; i++)
                {
                    var counter = group.Counters[i];
                    sheet.Cells.Add(new XlsxCell(0, ++row) { Value = counter.Name, Style = leftStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = counter.Value, Style = rightStyle });
                }

                ++row;
            }

            return sheet;
        }

        #endregion

        #region Summary

        private SummaryCounterData GetLearnersSummaryCounterData(IEnumerable<VLearnerActivity> list)
        {
            return GetSummaryCounterData(list, x => x.Select(y => y.LearnerIdentifier).Distinct().Count());
        }

        private SummaryCounterData GetActivitiesSummaryCounterData(IEnumerable<VLearnerActivity> list)
        {
            return GetSummaryCounterData(list, x => x.Count());
        }

        private SummaryCounterData GetSummaryCounterData(IEnumerable<VLearnerActivity> list, Func<IEnumerable<VLearnerActivity>, int> getCount)
        {
            return new SummaryCounterData
            {
                ProgramNames = list.GroupBy(x => x.ProgramName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                GradebookNames = list.GroupBy(x => x.GradebookName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EnrollmentStatuses = list.GroupBy(x => x.EnrollmentStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EngagementStatuses = list.GroupBy(x => x.EngagementStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerGenders = list.GroupBy(x => x.LearnerGender)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerReferrers = list.GroupBy(x => x.ReferrerName)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                ImmigrationStatuses = list.GroupBy(x => x.ImmigrationStatus)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                ImmigrationDestinations = list.GroupBy(x => x.ImmigrationDestination)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerCitizenships = list.GroupBy(x => x.LearnerCitizenship)
                    .Select(x => new Counter { Name = x.Key ?? "(None)", Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray()
            };
        }

        public void BindSummaryCounterData(SummaryCounterData data, VLearnerActivityFilter filter)
        {
            Filter = filter;

            ProgramNames.DataSource = data.ProgramNames;
            ProgramNames.DataBind();

            GradebookNames.DataSource = data.GradebookNames;
            GradebookNames.DataBind();

            EnrollmentStatuses.DataSource = data.EnrollmentStatuses;
            EnrollmentStatuses.DataBind();

            EngagementStatuses.DataSource = data.EngagementStatuses;
            EngagementStatuses.DataBind();

            LearnerGenders.DataSource = data.LearnerGenders;
            LearnerGenders.DataBind();

            LearnerReferrers.DataSource = data.LearnerReferrers;
            LearnerReferrers.DataBind();

            ImmigrationStatuses.DataSource = data.ImmigrationStatuses;
            ImmigrationStatuses.DataBind();

            ImmigrationDestinations.DataSource = data.ImmigrationDestinations;
            ImmigrationDestinations.DataBind();

            LearnerCitizenships.DataSource = data.LearnerCitizenships;
            LearnerCitizenships.DataBind();
        }

        #endregion

    }
}