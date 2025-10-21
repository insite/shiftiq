using System;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ReportTypeConstant = Shift.Constant.ReportType;

namespace InSite.UI.Admin.Reports
{
    public partial class Create : AdminBasePage
    {
        private bool AllowChangeType
        {
            get => (bool)(ViewState[nameof(AllowChangeType)] ?? false);
            set => ViewState[nameof(AllowChangeType)] = value;
        }

        private bool IsBuild
        {
            get => (bool)(ViewState[nameof(IsBuild)] ?? false);
            set => ViewState[nameof(IsBuild)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            ReportSelector.AutoPostBack = true;
            ReportSelector.ValueChanged += ReportSelector_ValueChanged;

            ReportType.AutoPostBack = true;
            ReportType.ValueChanged += ReportType_ValueChanged;

            JsonFileUploadExtensionValidator.ServerValidate += JsonFileUploadExtensionValidator_ServerValidate;
            JsonSchemaValidator.ServerValidate += JsonSchemaValidator_ServerValidate;
            JsonFileUploadButton.Click += JsonFileUploadButton_Click;

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
            PageHelper.AutoBindHeader(this, null, null);

            JsonSchemaValidator.ErrorMessage = Translate("Wrong JSON file uploaded");

            AllowChangeType = Organization.Identifier == OrganizationIdentifiers.Global;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            ReportSelector.ListFilter.IncludeShared = true;
            ReportSelector.ListFilter.ReportTypes = new[] { ReportTypeConstant.Custom, ReportTypeConstant.Shared };
            ReportSelector.RefreshData();

            ReportTypeField.Visible = AllowChangeType;

            ReportData.Value = Build.CreateReportJson;

            SetCreationType();
            OnCreationTypeChanged();
            OnReportTypeChanged();

            CancelButton.NavigateUrl = "/ui/admin/reports/search";
        }

        private void SetCreationType()
        {
            var action = Request.QueryString["action"];

            if (action.HasNoValue())
                return;

            IsBuild = (action == "build");

            if (action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;
                ReportSelector.ValueAsGuid = Guid.TryParse(Request["id"], out Guid reportId) ? reportId : (Guid?)null;
            }
        }

        private void CreationType_ValueChanged(object sender, EventArgs e) =>
            OnCreationTypeChanged();

        private void OnCreationTypeChanged()
        {
            var type = CreationType.ValueAsEnum;
            if (type == CreationTypeEnum.None)
                return;

            var isOne = type == CreationTypeEnum.One;
            var isDuplicate = type == CreationTypeEnum.Duplicate;
            var isUpload = type == CreationTypeEnum.Upload;

            JsonSection.Visible = isOne;
            ReportSelectorField.Visible = isDuplicate;
            JsonFileUploadContainer.Visible = isUpload;
            JsonInputField.Visible = isUpload;
            ReportInputsContainer.Visible = isOne || isDuplicate;
            ReportTypeField.Visible = !isDuplicate;
            PrivacyGroupsField.Visible = isOne;

            if (isOne && !IsBuild)
            {
                ReportTitle.Text = null;
                ReportData.Value = null;
                ReportDescription.Text = null;
            }
            else if (isDuplicate)
            {
                LoadDuplicate();
            }
        }

        private void ReportSelector_ValueChanged(object sender, EventArgs e)
        {
            if (ReportSelector.Value.IsNotEmpty())
                LoadDuplicate();
        }

        private void ReportType_ValueChanged(object sender, EventArgs e) => OnReportTypeChanged();

        private void OnReportTypeChanged()
        {
            PrivacyGroups.SetFilter(ReportType.Value);
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value == null || args.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        private void JsonSchemaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = VReportHelper.Deserialize(JsonInput.Text) != null;
        }

        private void JsonFileUploadButton_Click(object sender, EventArgs e)
        {
            if (JsonFileUpload.PostedFile == null || JsonFileUpload.PostedFile.ContentLength == 0)
                return;

            string text;
            using (var reader = new StreamReader(JsonFileUpload.FileContent, Encoding.UTF8))
                text = reader.ReadToEnd();

            JsonInput.Text = text;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var reportId = UniqueIdentifier.Create();

            var report = new TReport
            {
                ReportIdentifier = reportId,
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = User.UserIdentifier,
                ReportType = ReportTypeConstant.Custom,
                Created = DateTimeOffset.Now,
                CreatedBy = User.UserIdentifier,
                Modified = DateTimeOffset.Now,
                ModifiedBy = User.UserIdentifier
            };

            var type = CreationType.ValueAsEnum;

            if (type == CreationTypeEnum.Upload)
            {
                var result = VReportHelper.Deserialize(JsonInput.Text);
                report.ReportData = result.ReportData;
                report.ReportDescription = result.ReportDescription;
                report.ReportTitle = result.ReportTitle;
            }
            else
            {
                if (AllowChangeType)
                {
                    if (ReportType.Value == "Shared")
                        report.ReportType = ReportTypeConstant.Shared;
                    else
                        report.ReportType = ReportTypeConstant.Custom;
                }

                report.ReportTitle = ReportTitle.Text;
                report.ReportData = ReportData.Value;
                report.ReportDescription = ReportDescription.Text;
            }

            TReportStore.Insert(report);

            if (PrivacyGroupsField.Visible)
                PrivacyGroups.SaveData(reportId);

            HttpResponseHelper.Redirect($"/ui/admin/reports/edit?id={reportId}", true);
        }

        private void LoadDuplicate()
        {
            if (ReportSelector.ValueAsGuid == null)
                return;

            var report = VReportSearch.Select(ReportSelector.ValueAsGuid.Value);
            if (report == null)
                return;

            ReportTitle.Text = report.ReportTitle;
            ReportData.Value = report.ReportData;
            ReportDescription.Text = report.ReportDescription;

            PrivacyGroups.LoadData(report.ReportIdentifier);

            JsonSection.Visible = false;
        }
    }
}
