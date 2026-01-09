using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

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
    public partial class ChangeField : AdminBasePage, IHasParentLinkParameters
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

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var field = ServiceLocator.JournalSearch.GetJournalSetupField(JournalSetupFieldIdentifier, x => x.JournalSetup, x => x.JournalSetup.Event);

            if (field == null || field.JournalSetup.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(field.JournalSetup, User.TimeZone);

            LoadData(field);

            PageHelper.AutoBindHeader(this, null, header);

            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={field.JournalSetupIdentifier}&panel=fields";
        }

        private void LoadData(QJournalSetupField field)
        {
            JournalSetupIdentifier = field.JournalSetupIdentifier;

            var fieldType = field.FieldType.ToEnum<JournalSetupFieldType>();

            FieldType.Text = fieldType.GetDescription();
            IsRequired.SelectedValue = field.FieldIsRequired.ToString().ToLower();

            var content = ServiceLocator.ContentSearch.GetBlock(field.JournalSetupFieldIdentifier);

            LabelText.Text = content?[JournalSetupField.ContentLabels.LabelText].Text;
            HelpText.Text = content?[JournalSetupField.ContentLabels.HelpText].Text;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var isRequired = bool.Parse(IsRequired.SelectedValue);

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupFieldIdentifier) ?? new Shift.Common.ContentContainer();
            content[JournalSetupField.ContentLabels.LabelText].Text = LabelText.Text;
            content[JournalSetupField.ContentLabels.HelpText].Text = HelpText.Text;

            var commands = new List<Command>();

            commands.Add(new ChangeJournalSetupField(JournalSetupIdentifier, JournalSetupFieldIdentifier, isRequired));
            commands.Add(new ChangeJournalSetupFieldContent(JournalSetupIdentifier, JournalSetupFieldIdentifier, content));

            ServiceLocator.SendCommands(commands);

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
