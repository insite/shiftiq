using System;
using System.IO;
using System.Text;
using System.Web.UI;

using InSite.Admin.Events.Registrations.Reports;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Controls
{
    public partial class SearchDownload : BaseSearchDownload
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;

            ResetButton.Click += ResetButton_Click;

            Manager.NeedReport += Manager_NeedReport;
            Manager.ReportSelected += Manager_ReportSelected;

            ExportRegistrationReport.Click += ExportRegistrationReport_Click;
            ExportAverageAgeByAchievement.Click += ExportAverageAgeByAchievement_Click;
            ExportApprenticeScores.Click += ExportApprenticeScores_Click;
            ExportApprenticeCompletionRate.Click += ExportApprenticeCompletionRate_Click;
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

        private void ExportRegistrationReport_Click(object sender, EventArgs e)
        {
            if (!ValidateMaxRowCount("pdf"))
                return;

            var searchResults = (SearchResults)Finder.Results;
            var report = (RegistrationReport)LoadControl("../Reports/RegistrationReport.ascx");
            report.LoadReport(searchResults.Filter, true);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Registration Report", "RegistrationReport");
        }

        private void ExportAverageAgeByAchievement_Click(object sender, EventArgs e)
        {
            if (!ValidateMaxRowCount("pdf"))
                return;

            var searchResults = (SearchResults)Finder.Results;
            var report = (AverageAgeByAchievements)LoadControl("../Reports/AverageAgeByAchievements.ascx");
            report.LoadReport(searchResults.Filter);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Average Age by Achievements", "AverageAgeByAchievements");
        }

        private void ExportApprenticeScores_Click(object sender, EventArgs e)
        {
            if (!ValidateMaxRowCount("xlsx"))
                return;

            var searchResults = (SearchResults)Finder.Results;
            var data = ApprenticeScoresReport.GetXlsx(searchResults.Filter);
            Response.SendFile("ApprenticeScores", "xlsx", data);
        }

        private void ExportApprenticeCompletionRate_Click(object sender, EventArgs e)
        {
            if (!ValidateMaxRowCount("pdf"))
                return;

            var searchResults = (SearchResults)Finder.Results;
            var report = (ApprenticeCompletionRateReport)LoadControl("../Reports/ApprenticeCompletionRateReport.ascx");
            report.LoadReport(searchResults.Filter);
            PrintPDFReport(report, PageOrientationType.Portrait, 1400, 980, "Apprentice Completion Rate", "ApprenticeCompletionRate");
        }

        protected override void SetInputValues(Domain.Reports.Download settings) => Settings.SetInputValues(settings);

        protected override void GetInputValues() => Settings.GetInputValues(CurrentReport);

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
    }
}