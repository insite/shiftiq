using System;
using System.IO;
using System.Text;
using System.Web.UI;

using InSite.Admin.Records.Scores.Reports;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Scores.Controls
{
    public partial class SearchDownload : BaseSearchDownload
    {
        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;

            ResetButton.Click += ResetButton_Click;

            Manager.NeedReport += Manager_NeedReport;
            Manager.ReportSelected += Manager_ReportSelected;

            ExportTopStudents.Click += ExportTopStudents_Click;
            ExportTopStudentsXlsx.Click += ExportTopStudentsXlsx_Click;
            ExportMostImprovedStudents.Click += ExportMostImprovedStudents_Click;
            ExportMostImprovedStudentsXlsx.Click += ExportMostImprovedStudentsXlsx_Click;
            ExportByStudentPassingRate.Click += ExportByStudentPassingRate_Click;
            ExportByStudentPassingRateXlsx.Click += ExportByStudentPassingRateXlsx_Click;
            ExportLowestScoreStudents.Click += ExportLowestScoreStudents_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                SetState(ColumnTabs.GetState());
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
            {
                var reportId = CurrentReport.Identifier;
                if (reportId == Guid.Empty)
                    reportId = null;

                Manager.SetSelectedReport(reportId);
            }

            ColumnTabs.SetState(GetState());
        }

        #endregion

        #region Event handlers

        private void DownloadButton_Click(object sender, EventArgs e) => OnDownload();

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Manager.SetSelectedReport(null);

            LoadReport(CreateReport());
        }

        protected override void OnColumnsChanged() => ColumnTabs.LoadData(CurrentReport.Columns);

        private void Manager_NeedReport(object sender, BaseReportManager.NeedReportArgs args)
        {
            args.Report = CurrentReport;
        }

        private void Manager_ReportSelected(object sender, BaseReportManager.ReportArgs args)
        {
            if (args.Report is Download download)
                LoadReport(download);
            else
                LoadState(true);
        }

        private void ExportTopStudents_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = (TopStudents)LoadControl("../Reports/TopStudents.ascx");
            report.LoadReport(searchResults.Filter);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Top 10 Students", "TopStudents");
        }

        private void ExportTopStudentsXlsx_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            Response.SendFile("TopStudents", "xlsx", TopStudents.GetXlsx(searchResults.Filter));
        }

        private void ExportMostImprovedStudents_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = (MostImprovedStudents)LoadControl("../Reports/MostImprovedStudents.ascx");
            report.LoadReport(searchResults.Filter);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Top 10 Most Improved Students", "MostImprovedStudents");
        }

        private void ExportMostImprovedStudentsXlsx_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            Response.SendFile("MostImprovedStudents", "xlsx", MostImprovedStudents.GetXlsx(searchResults.Filter));
        }

        private void ExportByStudentPassingRate_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = (PassingRate)LoadControl("../Reports/PassingRate.ascx");
            report.LoadReport(searchResults.Filter);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Students Passing Rate", "PassingRate");
        }

        private void ExportByStudentPassingRateXlsx_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            Response.SendFile("PassingRate", "xlsx", PassingRate.GetXlsx(searchResults.Filter));
        }

        private void ExportLowestScoreStudents_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var data = LowestScoreStudentReport.GetXlsx(searchResults.Filter);
            Response.SendFile("lowest-score-students", "xlsx", data);
        }

        #endregion

        #region Methods (get/set input values)

        protected override void SetInputValues(Domain.Reports.Download settings) => Settings.SetInputValues(settings);

        protected override void GetInputValues() => Settings.GetInputValues(CurrentReport);

        #endregion

        #region Methods (helpers)

        private void PrintPDFReport(Control report, PageOrientationType pageOrientation, int width, int height, string reportName, string reportFileName)
        {
            var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = pageOrientation,
                Viewport = new HtmlConverterSettings.ViewportSize(width, height),
                MarginTop = 22,
                MarginBottom = 22,
                Dpi = 240,

                HeaderTextLeft = reportName,
                HeaderFontName = "Calibri",
                HeaderFontSize = 19,
                HeaderSpacing = 7.8f,

                FooterTextLeft = reportName,
                FooterTextCenter = date,
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "Calibri",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            Response.SendFile(reportFileName, "pdf", data);
        }

        #endregion
    }
}