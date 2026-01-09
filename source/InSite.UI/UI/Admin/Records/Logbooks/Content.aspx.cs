using System;

using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Logbooks
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier, x => x.Event);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search", true);
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier) ?? new Shift.Common.ContentContainer();

            ContentEditor.Add(new AssetContentSection.SingleLine(JournalSetupState.ContentLabels.Title)
            {
                Title = "Title",
                Label = "Title",
                Value = content[JournalSetupState.ContentLabels.Title].Text
            });

            ContentEditor.Add(new AssetContentSection.Markdown(JournalSetupState.ContentLabels.Instructions)
            {
                Title = "Instructions",
                Label = "Instructions",
                Value = content[JournalSetupState.ContentLabels.Instructions].Text,
                AllowUpload = true,
                UploadFolderPath = OrganizationRelativePath.JournalSetupPathTemplate.Format(journalSetup.JournalSetupIdentifier)
            });

            ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
            ContentEditor.OpenTab(Request["tab"]);

            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=setup";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier) ?? new Shift.Common.ContentContainer();
            content.Title.Text = ContentEditor.GetValue(JournalSetupState.ContentLabels.Title);
            content[JournalSetupState.ContentLabels.Instructions].Text = ContentEditor.GetValue(JournalSetupState.ContentLabels.Instructions);

            ServiceLocator.SendCommand(new ChangeJournalSetupContent(JournalSetupIdentifier, content));

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupIdentifier}&panel=setup", true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}&panel=setup"
                : null;
        }
    }
}
