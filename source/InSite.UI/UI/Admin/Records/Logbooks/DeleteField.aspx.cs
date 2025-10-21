using System;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks
{
    public partial class DeleteField : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupFieldIdentifier => Guid.TryParse(Request["field"], out var value) ? value : Guid.Empty;

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var field = ServiceLocator.JournalSearch.GetJournalSetupField(JournalSetupFieldIdentifier,
                    x => x.JournalSetup, 
                    x => x.JournalSetup.Event,
                    x => x.JournalSetup.Achievement,
                    x => x.JournalSetup.Framework);

                if (field == null || field.JournalSetup.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }
               

                LogbookName.Text = $"<a href=\"/ui/admin/records/logbooks/outline?journalsetup={field.JournalSetup.JournalSetupIdentifier}\">{field.JournalSetup.JournalSetupName}</a>";
                var content = ServiceLocator.ContentSearch.GetBlock(field.JournalSetup.JournalSetupIdentifier, MultilingualString.DefaultLanguage);
                var title = content?.Title?.Text.Default;
                LogbookTitle.Text = !string.IsNullOrEmpty(title) ? title : "N/A";

                LoadData(field);
                PageHelper.AutoBindHeader(this, null, LogbookHeaderHelper.GetLogbookHeader(field.JournalSetup, User.TimeZone));

                CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={field.JournalSetupIdentifier}&panel=fields";
            }
        }

        private void LoadData(QJournalSetupField field)
        {
            JournalSetupIdentifier = field.JournalSetupIdentifier;

            var fieldType = field.FieldType.ToEnum<JournalSetupFieldType>();

            FieldType.Text = fieldType.GetDescription();
            IsRequired.Text = field.FieldIsRequired ? "Yes" : "No";

            var content = ServiceLocator.ContentSearch.GetBlock(field.JournalSetupFieldIdentifier, Shift.Common.ContentContainer.DefaultLanguage);
            var labelText = content?[JournalSetupField.ContentLabels.LabelText].Text.Default;
            var helpText = content?[JournalSetupField.ContentLabels.HelpText].Text.Default;

            LabelText.Text = !string.IsNullOrEmpty(labelText) ? labelText : "N/A";
            HelpText.Text = !string.IsNullOrEmpty(helpText) ? helpText : "N/A";

            var journalCount = ServiceLocator.JournalSearch.CountJournals(new QJournalFilter
            {
                JournalSetupIdentifier = JournalSetupIdentifier,
            });

            DeleteJournalItems.Text = $"{journalCount:0}";

            if (journalCount > 0)
                DeleteConfirmationCheckbox.Text = "Delete this logbook's Field and all its values in Log Entries";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeleteJournalSetupField(JournalSetupIdentifier, JournalSetupFieldIdentifier));

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=fields");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=fields"
                : null;
        }
    }
}