using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class PrintSettingsManager : BaseReportManager
    {
        private static readonly string BasePersonalizationName = typeof(PrintSettingsManager).FullName + ":Base";
        private static readonly string DefaultPersonalizationName = typeof(PrintSettingsManager).FullName + ":Default";

        private string DocumentType
        {
            get => (string)ViewState[nameof(DocumentType)];
            set => ViewState[nameof(DocumentType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Script.ContentKey = UniqueID;

            SetDefaultButton.Click += SetDefaultButton_Click;
        }

        private void SetDefaultButton_Click(object sender, EventArgs e)
        {
            var filter = OnNeedReport();
            if (filter != null && filter is PrintSettings settings)
                SaveDefaultSettings(DocumentType, settings);
        }

        protected override ControlsInfo GetControls() => new ControlsInfo
        {
            ReportSelector = SavedFilterSelector,
            SaveButton = SaveFilterButton,
            RemoveButton = RemoveFilterButton,
            CreateButton = NewFilterButton,
            ReportName = NewFilterText,
        };

        protected override List<ISearchReport> LoadReports()
        {
            return PersonalizationRepository
                .GetValue<List<ISearchReport>>(Organization.OrganizationIdentifier, User.UserIdentifier, BasePersonalizationName, false)
                .EmptyIfNull()
                .ToList();
        }

        protected override void SaveReports(List<ISearchReport> value)
        {
            PersonalizationRepository.SetValue(Organization.OrganizationIdentifier, User.UserIdentifier, BasePersonalizationName, value);
        }

        protected override bool BindSavedReports()
        {
            var hasReports = base.BindSavedReports();

            SavedFilterField.Visible = hasReports;

            return hasReports;
        }

        public PrintSettings SetDocumentType(string documentType)
        {
            DocumentType = documentType;

            SetDefaultButton.Visible = true;
            SetDefaultButton.ToolTip = $"Set current settings as default for {documentType}";
            SetDefaultButton.ConfirmText = $"Are you sure you want to set current settings as default for {documentType}?";

            return LoadDefaultSettings(documentType);
        }

        public static PrintSettings LoadDefaultSettings(string documentType)
        {
            var container = LoadDefaultSettingsContainer();

            return container.ContainsKey(documentType) ? container[documentType] : null;
        }

        private static void SaveDefaultSettings(string documentType, PrintSettings settings)
        {
            var container = LoadDefaultSettingsContainer();

            container.Remove(documentType);

            if (settings != null)
                container.Add(documentType, settings);

            SaveDefaultSettingsContainer(container);
        }

        private static Dictionary<string, PrintSettings> LoadDefaultSettingsContainer() =>
            PersonalizationRepository.GetValue<Dictionary<string, PrintSettings>>(
                Organization.OrganizationIdentifier, Guid.Empty, DefaultPersonalizationName, false) ?? new Dictionary<string, PrintSettings>();

        private static void SaveDefaultSettingsContainer(Dictionary<string, PrintSettings> settings) =>
            PersonalizationRepository.SetValue(
                Organization.OrganizationIdentifier, Guid.Empty, DefaultPersonalizationName, settings);
    }
}