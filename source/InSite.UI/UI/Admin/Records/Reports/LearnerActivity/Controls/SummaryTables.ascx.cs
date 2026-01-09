using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Records.Reports.LearnerActivity.Models;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Reports.LearnerActivity.Controls
{
    public partial class SummaryTables : BaseUserControl
    {
        #region Events

        internal event SummaryTablesNeedDataSourceEventHandler NeedDataSource;

        private SummaryTablesDataSource OnNeedDataSource()
        {
            var args = new SummaryTablesNeedDataSourceArgs();

            NeedDataSource?.Invoke(this, args);

            return args.DataSource
                ?? throw ApplicationError.Create("DataSource is null");
        }

        #endregion

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
            var data = GetCounterData();

            var program = CreateSheet("Program", new[]
            {
                new SummaryExportDataGroup { Heading = "Program Name", Counters = data.ProgramNames },
                new SummaryExportDataGroup { Heading = "Gradebook  Name", Counters = data.GradebookNames },
                new SummaryExportDataGroup { Heading = "Gradebook Enrollment Status", Counters = data.EnrollmentStatuses }
            });
            var engagement = CreateSheet("Engagement", new[]
            {
                new SummaryExportDataGroup { Heading = "Sign In Activity", Counters = data.EngagementStatuses }
            });
            var learner = CreateSheet("Learner", new[]
            {
                new SummaryExportDataGroup { Heading = "Gender", Counters = data.LearnerGenders },
                new SummaryExportDataGroup { Heading = "Employed By / Belongs To", Counters = data.LearnerEmployers },
                new SummaryExportDataGroup { Heading = "Citizenship", Counters = data.LearnerCitizenships },
                new SummaryExportDataGroup { Heading = "Membership Status", Counters = data.MembershipStatuses }
            });

            return XlsxWorksheet.GetBytes(program, engagement, learner);
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

        private SummaryCounterData GetCounterData()
        {
            var dataSource = OnNeedDataSource();
            return dataSource.IsSummaryCountStrategy
                ? GetSummaryCounterData(dataSource.Items, x => x.Select(y => y.LearnerIdentifier).Distinct().Count())
                : GetSummaryCounterData(dataSource.Items, x => x.Count());
        }

        private SummaryCounterData GetSummaryCounterData(IEnumerable<SearchResultDataItem> list, Func<IEnumerable<SearchResultDataItem>, int> getCount)
        {
            return new SummaryCounterData
            {
                ProgramNames = list.Where(x => x.Programs.IsNotEmpty())
                    .SelectMany(x => x.Programs.Select(y => (ProgramName: y.Name, Item: x)))
                    .GroupBy(x => x.ProgramName)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x.Select(y => y.Item)) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                GradebookNames = list.GroupBy(x => x.GradebookName)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EnrollmentStatuses = list.GroupBy(x => x.EnrollmentStatus)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                EngagementStatuses = list.GroupBy(x => x.EngagementStatus)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerGenders = list.GroupBy(x => x.LearnerGender)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerEmployers = list.GroupBy(x => x.EmployerName)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                LearnerCitizenships = list.GroupBy(x => x.LearnerCitizenship)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray(),

                MembershipStatuses = list.GroupBy(x => x.MembershipStatus)
                    .Select(x => new Counter { Name = x.Key.IfNullOrEmpty("(None)"), Value = getCount(x) })
                    .OrderBy(x => x.Name)
                    .ToArray()
            };
        }

        public void BindSummaryCounterData()
        {
            var data = GetCounterData();

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

            LearnerEmployers.DataSource = data.LearnerEmployers;
            LearnerEmployers.DataBind();

            LearnerCitizenships.DataSource = data.LearnerCitizenships;
            LearnerCitizenships.DataBind();

            LearnerMembershipStatuses.DataSource = data.MembershipStatuses;
            LearnerMembershipStatuses.DataBind();
        }

        #endregion
    }
}