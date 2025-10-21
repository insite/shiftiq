using System;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Forms
{
    public partial class Analysis : AdminBasePage
    {
        #region Enums

        private enum ReportType
        {
            Unknown = 0,
            JobFitAnalysis1 = 1,
            JobFitAnalysis2 = 2,
            CareerMap = 3,
            StandardsAnalysis = 4
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportTypeSelector.AutoPostBack = true;
            ReportTypeSelector.ValueChanged += ReportTypeSelector_ValueChanged;

            InputsJobFit1.Update += InputsJobFit1_Update;
            InputsJobFit2.Update += InputsJobFit2_Update;
            InputsCareerMap.Update += InputsCareerMap_Update;
            InputsStandards.Update += InputsStandards_Update;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            OnReportTypeSelected();
        }

        #endregion

        #region Event handlers

        private void ReportTypeSelector_ValueChanged(object sender, EventArgs e) => OnReportTypeSelected();

        private void OnReportTypeSelected()
        {
            var reportType = ReportTypeSelector.Value.ToEnum(ReportType.Unknown);
            var reportName = string.Empty;

            switch (reportType)
            {
                case ReportType.JobFitAnalysis1:
                case ReportType.JobFitAnalysis2:
                    reportName = "Job Fit Analysis";
                    break;
                case ReportType.CareerMap:
                    reportName = "Career Map";
                    break;
                case ReportType.StandardsAnalysis:
                    reportName = "Standards Analysis";
                    break;
            }

            var isJobFit1 = reportType == ReportType.JobFitAnalysis1;
            var isJobFit2 = reportType == ReportType.JobFitAnalysis2;
            var isCareerMap = reportType == ReportType.CareerMap;
            var isStandards = reportType == ReportType.StandardsAnalysis;

            InputsJobFit1.Visible = isJobFit1;
            InputsJobFit1.SetDefaultValues();

            InputsJobFit2.Visible = isJobFit2;
            InputsJobFit2.SetDefaultValues();

            InputsCareerMap.Visible = isCareerMap;
            InputsCareerMap.SetDefaultValues();

            InputsStandards.Visible = isStandards;
            InputsStandards.SetDefaultValues();

            if (isJobFit1 || isJobFit2)
            {
                if (!StandardSearch.Exists(x => x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier && x.StandardType == Shift.Constant.StandardType.Profile))
                    ScreenStatus.AddMessage(AlertType.Warning, $"Competency Profile must be created before creating a {reportName}");
            }

            if (isJobFit1 || isCareerMap || isStandards)
            {
                if (!StandardSearch.Exists(x => x.OrganizationIdentifier == CurrentSessionState.Identity.Organization.Identifier && x.StandardType == Shift.Constant.StandardType.Document && x.DocumentType == DocumentType.NationalOccupationStandard))
                    ScreenStatus.AddMessage(AlertType.Warning, $"NOS document must be created before creating a {reportName}");
            }

            ReportColumn.Visible = false;
        }

        private void InputsJobFit1_Update(object sender, AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataJobFit1> e)
        {
            HideReport();

            ReportColumn.Visible = true;

            ReportTitle.InnerText = e.Data.Title;

            ReportJobFit1.LoadData(e.Data);
        }

        private void InputsJobFit2_Update(object sender, AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataJobFit2> e)
        {
            HideReport();

            ReportColumn.Visible = true;

            ReportTitle.InnerText = e.Data.Title;

            ReportJobFit2.LoadData(e.Data);
        }

        private void InputsCareerMap_Update(object sender, AnalysisHelper.UpdateEventArgs<AnalysisHelper.ReportDataCareerMap> e)
        {
            HideReport();

            ReportColumn.Visible = true;

            ReportTitle.InnerText = e.Data.Title;

            ReportCareerMap.LoadData(e.Data);
        }

        private void InputsStandards_Update(object sender, AnalysisHelper.UpdateEventArgs<AnalysisHelper.IReportDataStandardAnalysis> e)
        {
            HideReport();

            ReportColumn.Visible = true;

            ReportTitle.InnerText = e.Data.Title;

            ReportStandards.LoadData(e.Data);
        }

        #endregion

        #region Methods (report)

        private void HideReport()
        {
            ReportColumn.Visible = false;

            ReportJobFit1.UnloadData();
            ReportJobFit2.UnloadData();
            ReportCareerMap.UnloadData();
            ReportStandards.UnloadData();
        }

        #endregion
    }
}