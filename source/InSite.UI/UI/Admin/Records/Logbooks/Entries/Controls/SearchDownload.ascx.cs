using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Admin.Records.Logbooks.Entries.Reports;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
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

            EntrySummary.Click += EntrySummary_Click;
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

        private void EntrySummary_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = EntrySummaryReport.Create(searchResults.Filter);

            Response.SendFile("entries-summary", "xlsx", report);
        }

        protected override void SetInputValues(Download settings) => Settings.SetInputValues(settings);

        protected override void GetInputValues() => Settings.GetInputValues(CurrentReport);
    }
}