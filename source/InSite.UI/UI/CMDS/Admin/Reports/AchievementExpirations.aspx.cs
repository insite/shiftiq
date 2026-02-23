using System;
using System.Drawing;

using InSite.Application.Records.Read;
using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class ExpiredCredentials : AdminBasePage, ICmdsUserControl
    {
        #region Properties

        private ExpiredCredentialsSearchCriteria CurrentParameters
        {
            get => (ExpiredCredentialsSearchCriteria)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportButton.Click += ReportButton_Click;
            ClearButton.Click += ClearButton_Click;

            DownloadButton.Click += (s, a) => DownloadXlsx();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            AchievementType.EnsureDataBound();
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (!Page.IsValid)
                return;

            LoadReport();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            UserName.Text = null;
            UserEmail.Text = null;

            AchievementType.ClearSelection();

            AssetTitle.Text = null;
            ExpiredSince.Value = null;
            ExpiredBefore.Value = null;

            ReportTab.Visible = false;
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new ExpiredCredentialsSearchCriteria
            {
                UserName = UserName.Text,
                UserEmail = UserEmail.Text,
                AssetSubtype = AchievementType.Value,
                AssetTitle = AssetTitle.Text,
                ExpiredSince = ExpiredSince.Value,
                ExpiredBefore = ExpiredBefore.Value
            };

            var dataSource = ServiceLocator.AchievementSearch.GetExpiredCredentials(CurrentParameters, Organization.Identifier);

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();
        }

        private void DownloadXlsx()
        {
            if (CurrentParameters == null)
                return;

            var dataSource = ServiceLocator.AchievementSearch.GetExpiredCredentials(CurrentParameters, Organization.Identifier);
            if (dataSource.Length == 0)
                return;

            using (var excel = new ExcelPackage())
            {
                var blueBackgroundColor = ColorTranslator.FromHtml("#3d78d8");
                var cellOddColor = ColorTranslator.FromHtml("#f1f1f1");
                var cellEvenColor = ColorTranslator.FromHtml("#fafafa");

                #region Styles Definition

                // Style: Default

                {
                    var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                    defaultStyle.Font.Name = "Arial";
                    defaultStyle.Font.Size = 10;
                    defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                // Style: Header

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Header");
                    newStyle.Style.Font.Color.SetColor(Color.White);
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    newStyle.Style.Fill.BackgroundColor.SetColor(blueBackgroundColor);
                    newStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Style: Odd

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Odd");
                    newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    newStyle.Style.Fill.BackgroundColor.SetColor(cellOddColor);
                }

                // Style: Even

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Even");
                    newStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    newStyle.Style.Fill.BackgroundColor.SetColor(cellEvenColor);
                }

                #endregion

                var rowNumber = 1;

                #region Report Generation

                var sheet = excel.Workbook.Worksheets.Add(Route.Title);

                sheet.Column(1).Width = 20;
                sheet.Column(2).Width = 30;
                sheet.Column(3).Width = 12;
                sheet.Column(4).Width = 35;
                sheet.Column(5).Width = 12;
                sheet.Column(6).Width = 50;
                sheet.Column(7).Width = 20;
                sheet.Column(8).Width = 20;

                // Header

                {
                    var row = sheet.Row(rowNumber);
                    row.Height = 30;

                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = "User Name";
                    cell1.StyleName = "Header";

                    var cell2 = sheet.Cells[rowNumber, 2];
                    cell2.Value = "User Email";
                    cell2.StyleName = "Header";

                    var cell3 = sheet.Cells[rowNumber, 3];
                    cell3.Value = "Achievement Type";
                    cell3.StyleName = "Header";

                    var cell4 = sheet.Cells[rowNumber, 4];
                    cell4.Value = "Achievement Subtype";
                    cell4.StyleName = "Header";

                    var cell5 = sheet.Cells[rowNumber, 5];
                    cell5.Value = "Achievement Code";
                    cell5.StyleName = "Header";

                    var cell6 = sheet.Cells[rowNumber, 6];
                    cell6.Value = "Achievement Title";
                    cell6.StyleName = "Header";

                    var cell7 = sheet.Cells[rowNumber, 7];
                    cell7.Value = "Expired";
                    cell7.StyleName = "Header";
                    cell7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    var cell8 = sheet.Cells[rowNumber, 8];
                    cell8.Value = "Days Since Expiration";
                    cell8.StyleName = "Header";
                    cell8.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    rowNumber++;
                }

                var number = 1;

                foreach (var dataItem in dataSource)
                {
                    var cellStyle = number % 2 == 0 ? "Even" : "Odd";

                    var cell1 = sheet.Cells[rowNumber, 1];
                    cell1.Value = dataItem.ContactName;
                    cell1.StyleName = cellStyle;

                    var cell2 = sheet.Cells[rowNumber, 2];
                    cell2.Value = dataItem.ContactEmail;
                    cell2.StyleName = cellStyle;

                    var cell3 = sheet.Cells[rowNumber, 3];
                    cell3.Value = dataItem.AssetType;
                    cell3.StyleName = cellStyle;

                    var cell4 = sheet.Cells[rowNumber, 4];
                    cell4.Value = dataItem.AssetSubtype;
                    cell4.StyleName = cellStyle;

                    var cell5 = sheet.Cells[rowNumber, 5];
                    cell5.Value = dataItem.AssetCode;
                    cell5.StyleName = cellStyle;

                    var cell6 = sheet.Cells[rowNumber, 6];
                    cell6.Value = dataItem.AssetTitle;
                    cell6.StyleName = cellStyle;
                    cell6.Style.WrapText = true;

                    var cell7 = sheet.Cells[rowNumber, 7];
                    cell7.Value = dataItem.Expired.UtcDateTime;
                    cell7.StyleName = cellStyle;
                    cell7.Style.Numberformat.Format = "MMM d, yyyy hh:mm";
                    cell7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    var cell8 = sheet.Cells[rowNumber, 8];
                    cell8.Value = dataItem.DaysSinceExpiration;
                    cell8.StyleName = cellStyle;
                    cell8.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    number++;
                    rowNumber++;
                }

                #endregion

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 8];

                excel.Workbook.Properties.Title = Route.Title;
                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                ReportXlsxHelper.Export(Route.Title, excel);
            }
        }

        #endregion
    }
}