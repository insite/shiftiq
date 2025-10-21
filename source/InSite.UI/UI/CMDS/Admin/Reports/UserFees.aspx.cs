using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class UserFees : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        private class DateRange
        {
            public DateTime FromDate { get; set; }
            public DateTime ThruDate { get; set; }
        }

        private class GroupItem
        {
            public string Category { get; set; }
            public string CompanyName { get; set; }
            public List<CmdsReportHelper.BillableUser> List { get; set; }
            public int UserCount => List.Sum(x => x.UserCount);
            public decimal UnitPrice => List[0].UnitPrice;
            public decimal Amount => List.Sum(x => x.Amount);
        }

        private class ClassificationItem
        {
            public string Classification { get; set; }
            public List<GroupItem> Groups { get; set; }
            public int UserCount => Groups.Sum(x => x.UserCount);
            public decimal Amount => Groups.Sum(x => x.Amount);
        }

        #endregion

        #region Constants

        private class Style
        {
            private static readonly Color BlueColor = Color.FromArgb(17, 109, 182);

            public static readonly XlsxCellStyle HeaderStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, FontColor = BlueColor };
            public static readonly XlsxCellStyle ColumnHeaderStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, BackgroundColor = Color.FromArgb(238, 236, 225) };
            public static readonly XlsxCellStyle NumericStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            public static readonly XlsxCellStyle BoldStyle = new XlsxCellStyle { IsBold = true };
            public static readonly XlsxCellStyle GroupTotalNumericStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right, FontColor = BlueColor };
            public static readonly XlsxCellStyle ClassTotalNumericStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right, FontColor = BlueColor };
            public static readonly string IntFormat = "#,##0";
            public static readonly string MoneyFormat = "#,##0.00";
        }

        private const decimal DefaultPricePerUser = 3.6m;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var manager = ScriptManager.GetCurrent(Page);
            if (manager != null)
                manager.AsyncPostBackTimeout = 10 * 60;

            SearchButton.Click += SearchButton_Click;
            DownloadButton.Click += DownloadButton_Click;

            SummaryPerClassificationRepeater.ItemDataBound += SummaryRepeater_ItemDataBound;
            SummaryPerCompanyRepeater.ItemDataBound += SummaryRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                var lastMonth = DateTime.Today.AddMonths(-1);
                ReportYear.ValueAsInt = lastMonth.Year;
                ReportMonth.ValueAsInt = lastMonth.Month;
                UnitPricePerPeriodClassA.ValueAsDecimal = DefaultPricePerUser;

                CheckDataCreated();
            }
        }

        #endregion

        #region Event handlers

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!CheckDataCreated())
            {
                var dateRange = GetDateRange();

                CmdsReportHelper.PrepareInvoice(dateRange.FromDate, dateRange.ThruDate, UnitPricePerPeriodClassA.ValueAsDecimal ?? 0);

                CheckDataCreated();
            }

            InitData();

            DownloadButton.Visible = true;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            ExportSearchResultsToXlsx();
        }

        private void SummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var classificationItem = (ClassificationItem)e.Item.DataItem;

            var groupRepeater = (Repeater)e.Item.FindControl("GroupRepeater");
            groupRepeater.ItemDataBound += GroupRepeater_ItemDataBound;
            groupRepeater.DataSource = classificationItem.Groups;
            groupRepeater.DataBind();
        }

        private void GroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var groupItem = (GroupItem)e.Item.DataItem;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = groupItem.List;
            itemRepeater.DataBind();
        }

        #endregion

        #region Helper methods

        private bool CheckDataCreated()
        {
            var dateRange = GetDateRange();

            if (CmdsReportHelper.HasBillableUsersForInSite(dateRange.FromDate, dateRange.ThruDate))
            {
                SearchButton.Text = "Search";
                SearchButton.Icon = "far fa-search";
                return true;
            }

            SearchButton.Text = "Generate";
            SearchButton.Icon = "far fa-chart-bar";

            return false;
        }

        private void InitData()
        {
            PreviewSection1.Visible = true;
            PreviewSection2.Visible = true;

            PreviewSection1.IsSelected = true;

            var data = SelectData();

            var summaryPerClassification = CreateSummaryPerClassification(data);

            SummaryPerClassificationRepeater.DataSource = summaryPerClassification;
            SummaryPerClassificationRepeater.DataBind();

            SummaryPerClassificationUserCount.Text = $@"{summaryPerClassification.Sum(x => x.UserCount):n0}";
            SummaryPerClassificationAmount.Text = $@"{summaryPerClassification.Sum(x => x.Amount):n2}";

            var summaryPerCompany = CreateSummaryPerCompany(data);

            SummaryPerCompanyRepeater.DataSource = summaryPerCompany;
            SummaryPerCompanyRepeater.DataBind();

            SummaryPerCompanyUserCount.Text = $@"{summaryPerCompany.Sum(x => x.UserCount):n0}";
            SummaryPerCompanyAmount.Text = $@"{summaryPerCompany.Sum(x => x.Amount):n2}";
        }

        private List<CmdsReportHelper.BillableUser> SelectData()
        {
            var dateRange = GetDateRange();

            return CmdsReportHelper.SelectBillableUsersForInSite(
                dateRange.FromDate,
                dateRange.ThruDate,
                UnitPricePerPeriodClassA.ValueAsDecimal ?? 0,
                0,
                0
            );
        }

        private DateRange GetDateRange()
        {
            var date = new DateTime(ReportYear.ValueAsInt.Value, ReportMonth.ValueAsInt.Value, 1);

            var fromDate = new DateTime(date.Year, date.Month, 1);
            var thruDate = fromDate.AddMonths(1).AddDays(-1);

            return new DateRange { FromDate = fromDate, ThruDate = thruDate };
        }

        private List<ClassificationItem> CreateSummaryPerClassification(IEnumerable<CmdsReportHelper.BillableUser> data)
        {
            return data.GroupBy(x => x.BillingClassification)
                .Select(x => new ClassificationItem
                {
                    Classification = GetClassificationName(x.Key),
                    Groups = x.GroupBy(y => new { y.Category, y.UnitPrice })
                        .Select(y => new GroupItem
                        {
                            Category = y.Key.Category,
                            List = y.OrderBy(z => z.CompanyName).ThenBy(z => z.SharedCompanyCount).ToList()
                        })
                        .OrderBy(y => y.Category)
                        .ToList()
                })
                .OrderBy(x => x.Classification)
                .ToList();
        }

        private List<ClassificationItem> CreateSummaryPerCompany(IEnumerable<CmdsReportHelper.BillableUser> data)
        {
            return data.GroupBy(x => x.BillingClassification)
                .Select(x => new ClassificationItem
                {
                    Classification = GetClassificationName(x.Key),
                    Groups = x.GroupBy(y => new { y.CompanyName, y.Category, y.UnitPrice })
                        .Select(y => new GroupItem
                        {
                            CompanyName = y.Key.CompanyName,
                            Category = y.Key.Category,
                            List = y.OrderBy(z => z.SharedCompanyCount).ToList()
                        })
                        .OrderBy(y => y.CompanyName).ThenBy(y => y.Category)
                        .ToList()
                })
                .OrderBy(x => x.Classification)
                .ToList();
        }

        private string GetClassificationName(string classification)
        {
            if (classification.Equals("A", StringComparison.OrdinalIgnoreCase))
                return "CLASS A USERS: Profiles and time-sensitive training requirements";

            return classification.Equals("B", StringComparison.OrdinalIgnoreCase)
                ? "CLASS B USERS: Time-sensitive training requirements only (no profiles)"
                : "CLASS C USERS: No profiles or time-sensitive training requirements";
        }

        #endregion

        #region Helper methods: export (XLSX)

        private void ExportSearchResultsToXlsx()
        {
            var data = SelectData();

            var xlsxSheet = new XlsxWorksheet("CMDS User Fees - Invoice Addendum");
            xlsxSheet.Columns[0].Width = 60;
            xlsxSheet.Columns[1].Width = 40;
            xlsxSheet.Columns[2].Width = 20;
            xlsxSheet.Columns[3].Width = 20;
            xlsxSheet.Columns[4].Width = 20;

            var rowNumber = AddSummaryPerClassificationToXlsx(xlsxSheet, data, 0);

            AddSummaryPerCompanyToXlsx(xlsxSheet, data, rowNumber + 1);

            ReportXlsxHelper.Export(xlsxSheet);
        }

        private int AddSummaryPerClassificationToXlsx(XlsxWorksheet xlsxSheet, IEnumerable<CmdsReportHelper.BillableUser> data, int rowNumber)
        {
            var classifications = CreateSummaryPerClassification(data);

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Style = Style.HeaderStyle, Value = "SUMMARY PER CLASSIFICATION" });

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Classification", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = "# Users", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Unit Price", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = "Amount", Style = Style.ColumnHeaderStyle });

            rowNumber++;

            foreach (var classificationItem in classifications)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Style = Style.HeaderStyle, Value = classificationItem.Classification });

                foreach (var groupItem in classificationItem.Groups)
                {
                    xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = groupItem.Category, Style = Style.BoldStyle });
                    xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = groupItem.UserCount, Style = Style.NumericStyle, Format = Style.IntFormat });
                    xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = groupItem.UnitPrice, Style = Style.NumericStyle, Format = Style.MoneyFormat });
                    xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = groupItem.Amount, Style = Style.NumericStyle, Format = Style.MoneyFormat });

                    rowNumber++;

                    foreach (var dataItem in groupItem.List)
                    {
                        xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = dataItem.CompanyName });
                        xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = dataItem.UserCount, Style = Style.NumericStyle, Format = Style.IntFormat });
                        xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = dataItem.Amount, Style = Style.NumericStyle, Format = Style.MoneyFormat });

                        rowNumber++;
                    }
                }

                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = classificationItem.UserCount, Style = Style.GroupTotalNumericStyle, Format = Style.IntFormat });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = classificationItem.Amount, Style = Style.GroupTotalNumericStyle, Format = Style.MoneyFormat });

                rowNumber++;
            }

            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = classifications.Sum(x => x.UserCount), Style = Style.ClassTotalNumericStyle, Format = Style.IntFormat });
            xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = classifications.Sum(x => x.Amount), Style = Style.ClassTotalNumericStyle, Format = Style.MoneyFormat });

            rowNumber++;

            return rowNumber;
        }

        private void AddSummaryPerCompanyToXlsx(XlsxWorksheet xlsxSheet, IEnumerable<CmdsReportHelper.BillableUser> data, int rowNumber)
        {
            var classifications = CreateSummaryPerCompany(data);

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Style = Style.HeaderStyle, Value = "SUMMARY PER ORGANIZATION" });

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Organization", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = "Worker/Organization Allocation", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "# Users", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = "Unit Price", Style = Style.ColumnHeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = "Amount", Style = Style.ColumnHeaderStyle });

            rowNumber++;

            foreach (var classificationItem in classifications)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 5) { Style = Style.HeaderStyle, Value = classificationItem.Classification });

                foreach (var groupItem in classificationItem.Groups)
                {
                    xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = groupItem.CompanyName, Style = Style.BoldStyle });
                    xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = groupItem.Category });
                    xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = groupItem.UserCount, Style = Style.NumericStyle, Format = Style.IntFormat });
                    xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = groupItem.UnitPrice, Style = Style.NumericStyle, Format = Style.MoneyFormat });
                    xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = groupItem.Amount, Style = Style.NumericStyle, Format = Style.MoneyFormat });

                    rowNumber++;

                    foreach (var dataItem in groupItem.List)
                    {
                        xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = dataItem.CompanyName });
                        xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = dataItem.Category });
                        xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = dataItem.UserCount, Style = Style.NumericStyle, Format = Style.IntFormat });
                        xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = dataItem.UnitPrice, Style = Style.NumericStyle, Format = Style.MoneyFormat });
                        xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = dataItem.Amount, Style = Style.NumericStyle, Format = Style.MoneyFormat });

                        rowNumber++;
                    }
                }

                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = classificationItem.UserCount, Style = Style.GroupTotalNumericStyle, Format = Style.IntFormat });
                xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = classificationItem.Amount, Style = Style.GroupTotalNumericStyle, Format = Style.MoneyFormat });

                rowNumber++;
            }

            xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = classifications.Sum(x => x.UserCount), Style = Style.ClassTotalNumericStyle, Format = Style.IntFormat });
            xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = classifications.Sum(x => x.Amount), Style = Style.ClassTotalNumericStyle, Format = Style.MoneyFormat });

            // rowNumber++;
            // return rowNumber;
        }

        #endregion
    }
}
