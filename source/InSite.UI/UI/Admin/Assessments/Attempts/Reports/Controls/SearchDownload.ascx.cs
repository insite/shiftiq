using System;

using InSite.Admin.Attempts.Reports.Downloads;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;

namespace InSite.Admin.Attempts.Reports.Controls
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

            AttemptsQTIButton.Click += AttemptsQTIButton_Click;
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

        private void AttemptsQTIButton_Click(object sender, EventArgs e)
        {
            var searchResults = (SearchResults)Finder.Results;
            var data = QTIWriter.Create(searchResults.Filter);
            Response.SendFile("Attempts", "xml", data);
        }

        #endregion

        #region Methods (get/set input values)

        protected override void SetInputValues(Domain.Reports.Download settings) => Settings.SetInputValues(settings);

        protected override void GetInputValues() => Settings.GetInputValues(CurrentReport);

        #endregion
    }
}