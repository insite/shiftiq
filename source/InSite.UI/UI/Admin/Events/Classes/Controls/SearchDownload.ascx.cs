using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Admin.Events.Classes.Reports;

using Shift.Common;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class SearchDownload : BaseSearchDownload
    {
        #region Classes

        [Serializable]
        private class DownloadInternal : Download
        {
            public bool IsBreakdownRegistrationCount { get; set; }

            public override void Reset()
            {
                base.Reset();

                IsBreakdownRegistrationCount = false;
            }
        }

        #endregion

        #region Fields

        private static readonly Type _reportType = typeof(DownloadInternal);

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetReportType(_reportType);

            DownloadButton.Click += (s, a) => OnDownload();

            ResetButton.Click += ResetButton_Click;

            Manager.SetReportType(_reportType);
            Manager.NeedReport += Manager_NeedReport;
            Manager.ReportSelected += Manager_ReportSelected;

            ExportByGroupStatus.Click += (s, a) => PrintPdfReport("../Reports/ClassRegistrationsByGroupStatus.ascx");
            ExportByAchievement.Click += (s, a) => PrintPdfReport("../Reports/ClassRegistrationsByAchievements.ascx");
            ExportByAchievementXlsx.Click += (s, a) => PrintXlsxReport("../Reports/ClassRegistrationsByAchievements.ascx");
            ExportByTrade.Click += (s, a) => PrintPdfReport("../Reports/ClassRegistrationsByTrade.ascx");
            ExportByTradeXlsx.Click += (s, a) => PrintXlsxReport("../Reports/ClassRegistrationsByTrade.ascx");
            ExportByCertificateaAndVenue.Click += (s, a) => PrintPdfReport("../Reports/ClassRegistrationsByCertificatesAndVenues.ascx");
            ExportByCertificateaAndVenueXlsx.Click += (s, a) => PrintXlsxReport("../Reports/ClassRegistrationsByCertificatesAndVenues.ascx");
            ExportStudentsByTrade.Click += (s, a) => PrintPdfReport("../Reports/ClassCountsByTrade.ascx");
            ExportStudentsByTradeXlsx.Click += (s, a) => PrintXlsxReport("../Reports/ClassCountsByTrade.ascx");
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

        protected override List<DownloadColumn> GetVisibleColumns(IList dataSource)
        {
            var columns = base.GetVisibleColumns(dataSource);

            if (CurrentReport is DownloadInternal report && report.IsBreakdownRegistrationCount && dataSource is DataView dataView)
            {
                var colName = nameof(SearchResults.ExportDataItem.RegistrationsCount);
                var colIndex = columns.FindIndex(x => x.Name == colName);

                if (colIndex >= 0)
                {
                    columns.RemoveAt(colIndex);

                    foreach (DataColumn dataCol in dataView.Table.Columns)
                    {
                        if (!dataCol.ColumnName.StartsWith(SearchResults.RegistrationStatusCountColumnPrefix))
                            continue;

                        var status = dataCol.ColumnName.Substring(SearchResults.RegistrationStatusCountColumnPrefix.Length);
                        var statusColTitle = report.RemoveSpaces
                            ? "RegistrationsCount" + status.IfNullOrEmpty("Null")
                            : "Registrations Count (" + status.IfNullOrEmpty("NULL") + ")";

                        columns.Insert(colIndex++, new DownloadColumn(dataCol.ColumnName, statusColTitle));
                    }
                }
            }

            return columns;
        }

        #endregion

        #region Methods (get/set input values)

        protected override void SetInputValues(Domain.Reports.Download settings)
        {
            Settings.SetInputValues(settings);

            var report = settings as DownloadInternal ?? new DownloadInternal();
            IsBreakdownRegistrationCount.Checked = report.IsBreakdownRegistrationCount;
        }

        protected override void GetInputValues()
        {
            Settings.GetInputValues(CurrentReport);

            var report = (DownloadInternal)CurrentReport;
            report.IsBreakdownRegistrationCount = IsBreakdownRegistrationCount.Checked;
        }

        #endregion

        #region Methods (helpers)

        private void PrintPdfReport(string reportPath)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = (BaseReportControl)LoadControl(reportPath);

            Response.SendFile(report.ReportFileName, "pdf", report.GetPdf(searchResults.Filter));
        }

        private void PrintXlsxReport(string reportPath)
        {
            var searchResults = (SearchResults)Finder.Results;
            var report = (BaseReportControl)LoadControl(reportPath);

            Response.SendFile(report.ReportFileName, "xlsx", report.GetXlsx(searchResults.Filter));
        }

        #endregion
    }
}