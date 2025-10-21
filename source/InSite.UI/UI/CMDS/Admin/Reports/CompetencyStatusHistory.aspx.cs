using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI.Chart;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using BarChart = InSite.Common.Web.UI.Chart.BarChart;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class CompetencyStatusHistoryChart : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        protected enum ChartTypeEnum { Lines, StackedBars, StackedArea }

        [Serializable]
        protected class SearchParameters
        {
            public Guid? OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public Guid? UserIdentifier { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public ICollection<string> StatusVisibility { get; set; }
            public ChartTypeEnum ChartType { get; set; }
            public string Option { get; set; }
            public bool IsSingleChart { get; set; }
        }

        private class ChartInfo : IComparable<ChartInfo>
        {
            #region Properties

            public string Company { get; set; }

            public string Department { get; set; }

            public string Employee { get; set; }

            public string Title =>
                Company
                + (string.IsNullOrEmpty(Department) ? string.Empty : $" ({Department})");

            public IReadOnlyList<ChartDataItem> Items { get; private set; }

            #endregion

            #region Construction

            public ChartInfo(IEnumerable<CmdsReportHelper.CompetencyStatusHistoryChart> rows)
            {
                Items = rows
                    .GroupBy(x => x.SnapshotDate)
                    .Select(x => new ChartDataItem
                    {
                        SnapshotDate = x.Key,
                        Expired = x.Average(y => y.CompetencyPercentExpired),
                        NotCompleted = x.Average(y => y.CompetencyPercentNotCompleted),
                        NotApplicable = x.Average(y => y.CompetencyPercentNotApplicable),
                        NeedsTraining = x.Average(y => y.CompetencyPercentNeedsTraining),
                        SelfAssessed = x.Average(y => y.CompetencyPercentSelfAssessed),
                        Submitted = x.Average(y => y.CompetencyPercentSubmitted),
                        Validated = x.Average(y => y.CompetencyPercentValidated),
                    })
                    .OrderBy(x => x.SnapshotDate)
                    .ToArray();
            }

            #endregion

            #region Methods

            public int CompareTo(ChartInfo other)
            {
                var result = Company.CompareTo(other.Company);

                if (result == 0)
                {
                    var thisHasDept = !string.IsNullOrEmpty(Department);
                    var otherHasDept = !string.IsNullOrEmpty(other.Department);

                    if (thisHasDept && otherHasDept)
                        result = Department.CompareTo(other.Department);
                    else if (otherHasDept)
                        result = -1;
                    else
                        result = 1;
                }

                return result;
            }

            #endregion
        }

        private class ChartDataItem
        {
            public DateTime SnapshotDate { get; set; }
            public decimal Expired { get; set; }
            public decimal NotCompleted { get; set; }
            public decimal NotApplicable { get; set; }
            public decimal NeedsTraining { get; set; }
            public decimal SelfAssessed { get; set; }
            public decimal Submitted { get; set; }
            public decimal Validated { get; set; }
        }

        #endregion

        #region Properties

        protected SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifierValidator.ServerValidate += (s, a) => a.IsValid = DepartmentIdentifier.Enabled;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => UpdateEmployeeIdentifier();

            ChartRepeater.ItemCreated += ChartRepeater_ItemCreated;
            ChartRepeater.ItemDataBound += ChartRepeater_ItemDataBound;

            DownloadPdf.Click += DownloadPdf_Click;

            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";

            UpdateEmployeeIdentifier();

            DateSince.Value = new DateTime(DateTime.Now.Year, 1, 1);
            DateBefore.Value = DateTime.Today;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadPdf_Click(object sender, EventArgs e)
        {
            var reportHtml = ReportHtmlContent.Value;
            if (string.IsNullOrEmpty(reportHtml))
                return;

            var bodyHtml = GetFileContent("~/UI/CMDS/Admin/Reports/CompetencyStatusHistoryChart_PdfBody.html");
            bodyHtml = bodyHtml
                .Replace("<!-- TITLE -->", Route.Title)
                .Replace("<!-- BODY -->", reportHtml);
            bodyHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, bodyHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = PageOrientationType.Landscape,
                Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/CompetencyStatusHistoryChart_PdfHeader.html"),
                HeaderSpacing = 8,
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(Route.Title, '-', false);

            Response.SendFile(filename, "pdf", data);

            string GetFileContent(string virtualPath)
            {
                var physPath = MapPath(virtualPath);

                return File.ReadAllText(physPath);
            }
        }

        private void UpdateEmployeeIdentifier()
        {
            EmployeeIdentifier.Value = null;
            EmployeeIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            EmployeeIdentifier.Filter.Departments = DepartmentIdentifier.Values;

            if (EmployeeIdentifier.Filter.Departments.Length == 0)
                EmployeeIdentifier.Filter.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            var hasData = EmployeeIdentifier.Filter.Departments.Length > 0
                && EmployeeIdentifier.GetCount() > 0;

            EmployeeIdentifier.Filter.RoleType = new[] { MembershipType.Department, MembershipType.Organization };

            EmployeeIdentifier.Enabled = hasData;
            EmployeeIdentifier.EmptyMessage = hasData ? "All Learners" : "None";
        }

        private void ChartRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            BaseChart baseChart;

            if (CurrentParameters.ChartType == ChartTypeEnum.Lines || CurrentParameters.ChartType == ChartTypeEnum.StackedArea)
            {
                var lineChart = new LineChart
                {
                    ID = "Chart",
                    DatasetType = LineChatDatasetType.DateTime,
                    ToolTipIntersect = false,
                    OnClientPreInit = "report.onChartPreInit",
                };
                lineChart.Options.Animation.Duration = 0;
                lineChart.Options.Plugins.Tooltip.Callbacks.LabelJsFunction = "report.onChartTooltipLabelCallback";

                baseChart = lineChart;
            }
            else if (CurrentParameters.ChartType == ChartTypeEnum.StackedBars)
            {
                var barChart = new BarChart
                {
                    ID = "Chart",
                    DatasetType = LineChatDatasetType.DateTime,
                    ToolTipIntersect = false,
                    OnClientPreInit = "report.onChartPreInit",
                };
                barChart.Options.Animation.Duration = 0;
                barChart.Options.Plugins.Tooltip.Callbacks.LabelJsFunction = "report.onChartTooltipLabelCallback";

                baseChart = barChart;
            }
            else
                throw new NotImplementedException();

            e.Item.FindControl("ChartPlaceholder").Controls.Add(baseChart);
        }

        private void ChartRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var data = (ChartInfo)e.Item.DataItem;
            var chart = (BaseChart)e.Item.FindControl("Chart");

            chart.Data.Clear();

            var expiredDataset = CreateLineChartDataset("expired", "Expired", Color.Black);
            var notCompletedDataset = CreateLineChartDataset("not_completed", "Not Completed", Color.FromArgb(128, 128, 128));
            var notApplicableDataset = CreateLineChartDataset("not_applicable", "Not Applicable", Color.RoyalBlue);
            var needsTrainingDataset = CreateLineChartDataset("needs_training", "Needs Training", Color.Orange);
            var selfAssessedDataset = CreateLineChartDataset("self_assessed", "Self-Assessed", Color.Firebrick);
            var submittedDataset = CreateLineChartDataset("submitted", "Submitted for Validation", Color.Gold);
            var validatedDataset = CreateLineChartDataset("validated", "Validated", Color.SeaGreen);

            foreach (var item in data.Items)
            {
                expiredDataset?.NewItem(item.SnapshotDate, (double)item.Expired);
                notCompletedDataset?.NewItem(item.SnapshotDate, (double)item.NotCompleted);
                notApplicableDataset?.NewItem(item.SnapshotDate, (double)item.NotApplicable);
                needsTrainingDataset?.NewItem(item.SnapshotDate, (double)item.NeedsTraining);
                selfAssessedDataset?.NewItem(item.SnapshotDate, (double)item.SelfAssessed);
                submittedDataset?.NewItem(item.SnapshotDate, (double)item.Submitted);
                validatedDataset?.NewItem(item.SnapshotDate, (double)item.Validated);
            }

            DateTimeChartDataset CreateLineChartDataset(string id, string label, Color color)
            {
                if (!CurrentParameters.StatusVisibility.Contains(id))
                    return null;

                var dataset = (DateTimeChartDataset)chart.Data.CreateDataset(id);
                dataset.Label = label;
                dataset.BackgroundColor = color;
                dataset.Fill = CurrentParameters.ChartType == ChartTypeEnum.StackedArea;

                if (CurrentParameters.ChartType != ChartTypeEnum.StackedBars)
                {
                    dataset.BorderColor = color;
                    dataset.BorderWidth = 2;
                    dataset.LineTension = 0.05M;
                    dataset.PointRadius = 1;
                }

                return dataset;
            }
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                Departments = DepartmentIdentifier.Values,
                UserIdentifier = EmployeeIdentifier.Value,
                StartDate = DateSince.Value.Value,
                EndDate = DateBefore.Value.Value,
                StatusVisibility = new HashSet<string>(StatusVisibility.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value)),
                ChartType = ChartType.SelectedValue.ToEnum<ChartTypeEnum>(),
                Option = Option.SelectedValue,
                IsSingleChart = IsSingleChart.Checked,
            };

            if (CurrentParameters.Departments.Length == 0)
                CurrentParameters.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            var databaseRows = CmdsReportHelper.SelectZUserStatusHistory(
                CurrentParameters.OrganizationIdentifier,
                CurrentParameters.Departments,
                CurrentParameters.UserIdentifier,
                CurrentParameters.StartDate,
                CurrentParameters.EndDate,
                CurrentParameters.Option);

            if (!databaseRows.Any())
            {
                ScreenStatus.AddMessage(AlertType.Information, "There is no data matching your criteria.");
                return;
            }

            var chartData = new List<ChartInfo>();
            var employeeName = CurrentParameters.UserIdentifier.HasValue ? databaseRows.FirstOrDefault()?.EmployeeName : null;

            if (CurrentParameters.IsSingleChart)
            {
                foreach (var companyGroup in databaseRows.GroupBy(x => x.OrganizationIdentifier))
                {
                    var firstGroupRow = companyGroup.First();

                    chartData.AddSorted(new ChartInfo(companyGroup)
                    {
                        Company = firstGroupRow.CompanyName,
                        Department = string.Join(", ", companyGroup.Select(x => x.DepartmentName).Distinct().OrderBy(x => x)),
                        Employee = employeeName
                    });
                }
            }
            else
            {
                if (CurrentParameters.Departments.Length == DepartmentIdentifier.GetCount())
                {
                    foreach (var companyGroup in databaseRows.GroupBy(x => x.OrganizationIdentifier))
                    {
                        var firstGroupRow = companyGroup.First();

                        chartData.AddSorted(new ChartInfo(companyGroup)
                        {
                            Company = firstGroupRow.CompanyName,
                            Employee = employeeName
                        });
                    }
                }

                foreach (var departmentGroup in databaseRows.GroupBy(x => x.DepartmentIdentifier))
                {
                    var firstGroupRow = departmentGroup.First();

                    chartData.AddSorted(new ChartInfo(departmentGroup)
                    {
                        Company = firstGroupRow.CompanyName,
                        Department = firstGroupRow.DepartmentName,
                        Employee = employeeName
                    });
                }
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;
            DownloadPdf.Visible = chartData.Count > 0;

            ChartRepeater.DataSource = chartData;
            ChartRepeater.DataBind();
        }

        #endregion
    }
}