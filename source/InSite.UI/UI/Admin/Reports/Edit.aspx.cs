using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ReportTypeConstant = Shift.Constant.ReportType;

namespace InSite.UI.Admin.Reports
{
    public partial class Edit : AdminBasePage
    {
        private Guid? ReportIdentifier => Guid.TryParse(Request["id"], out Guid result) ? result : (Guid?)null;

        private bool AllowChangeType
        {
            get => (bool)(ViewState[nameof(AllowChangeType)] ?? false);
            set => ViewState[nameof(AllowChangeType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportType.AutoPostBack = true;
            ReportType.ValueChanged += ReportType_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var report = ReportIdentifier.HasValue ? VReportSearch.Select(ReportIdentifier.Value) : null;

            if (report == null)
                HttpResponseHelper.SendHttp404();

            if (!VReportSearch.HasPermissions(report, Organization.Identifier, User.UserIdentifier))
                HttpResponseHelper.SendHttp403();

            PageHelper.AutoBindHeader(this, null, report.ReportTitle);

            var isCustom = StringHelper.EqualsAny(report.ReportType, new[] { ReportTypeConstant.Custom, ReportTypeConstant.Shared });
            var isSameOrganization = report.OrganizationIdentifier == Organization.Identifier;
            var isGlobalOrganization = Organization.Identifier == OrganizationIdentifiers.Global;

            AllowChangeType = isCustom && isGlobalOrganization;

            if (AllowChangeType)
            {
                var reportType = StringHelper.Equals(report.ReportType, ReportTypeConstant.Custom) ? "Custom" : "Shared";

                ReportType.Value = reportType;

                PrivacyGroups.SetFilter(reportType);
            }

            ReportTitle.Text = report.ReportTitle;
            ReportData.Value = report.ReportData;
            ReportDescription.Text = report.ReportDescription;

            if (isSameOrganization)
                PrivacyGroups.LoadData(report.ReportIdentifier);

            ReportTypeField.Visible = AllowChangeType;
            GroupsField.Visible = isSameOrganization;
            JsonSection.Visible = isSameOrganization;
            CustomReportButtons.Visible = isCustom;

            SetupButtons(report, isSameOrganization, isCustom);
        }

        private void SetupButtons(VReport report, bool isSameOrganization, bool isCustom)
        {
            SaveButton.Visible = isSameOrganization;

            CancelButton.NavigateUrl = "/ui/admin/reports/search";
            CancelButton.Visible = isSameOrganization;

            DuplicateLink.NavigateUrl = $"/ui/admin/reports/create?action=duplicate&id={report.ReportIdentifier}";

            DownloadLink.NavigateUrl = $"/ui/admin/reports/download?id={report.ReportIdentifier}";
            DownloadLink.Visible = isSameOrganization;

            ExecuteButton.Visible = isCustom;
            ExecuteButton.NavigateUrl = $"/ui/admin/reports/build?id={ReportIdentifier}&execute=true";

            BuildButton.Visible = isCustom && isSameOrganization;
            BuildButton.NavigateUrl = $"/ui/admin/reports/build?id={ReportIdentifier}";

            CloseButton.NavigateUrl = "/ui/admin/reports/search";
            CloseButton.Visible = !isSameOrganization;
        }

        private void ReportType_ValueChanged(object sender, EventArgs e) => OnReportTypeChanged();

        private void OnReportTypeChanged() => PrivacyGroups.SetFilter(ReportType.Value);

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var report = TReportSearch.Select(ReportIdentifier.Value);

            if (AllowChangeType)
            {
                if (ReportType.Value == "Custom")
                    report.ReportType = ReportTypeConstant.Custom;
                else if (ReportType.Value == "Shared")
                    report.ReportType = ReportTypeConstant.Shared;
            }

            if (JsonSection.Visible)
                report.ReportData = ReportData.Value;

            report.ReportTitle = ReportTitle.Text;
            report.ReportDescription = ReportDescription.Text;

            report.Modified = DateTimeOffset.Now;
            report.ModifiedBy = User.UserIdentifier;

            TReportStore.Update(report);

            if (GroupsField.Visible)
                PrivacyGroups.SaveData(report.ReportIdentifier);

            HttpResponseHelper.Redirect("/ui/admin/reports/search", true);
        }
    }
}