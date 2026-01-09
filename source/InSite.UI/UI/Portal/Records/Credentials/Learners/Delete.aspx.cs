using System;
using System.Linq;

using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Records.Credentials.Learners
{
    public partial class Delete : PortalBasePage
    {
        private Guid CredentialIdentifier => Guid.TryParse(Request["credential"], out var id) ? id : Guid.Empty;

        private Guid? FileIdentifier
        {
            get
            {
                return ViewState[nameof(FileIdentifier)] as Guid?;
            }
            set
            {
                ViewState[nameof(FileIdentifier)] = value;
            }
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
                LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (DeleteCredential.Checked)
            {
                var command = new DeleteCredential(CredentialIdentifier);

                ServiceLocator.SendCommand(command);
            }

            if (FileIdentifier.HasValue && (DeleteCredential.Checked || DeleteAttachment.Checked))
            {
                var storage = ServiceLocator.StorageService;

                storage.Delete(FileIdentifier.Value);
            }

            HttpResponseHelper.Redirect(GetSearchUrl());
        }

        private void LoadData()
        {
            var credential = ServiceLocator.AchievementSearch.GetCredential(CredentialIdentifier);
            if (credential == null)
                RedirectToSearch();

            CancelButton.NavigateUrl = GetSearchUrl();

            PageHelper.AutoBindHeader(this, qualifier: credential.AchievementTitle);

            LearnerName.Text = $"{credential.UserFirstName} {credential.UserLastName}";

            AchievementType.Text = credential.AchievementLabel;

            if (credential.CredentialGranted.HasValue)
                AchievementDate.Text = TimeZones.FormatDateOnly(credential.CredentialGranted.Value, User.TimeZone);

            BindAttachmentField();
        }

        private void BindAttachmentField()
        {
            var storage = ServiceLocator.StorageService;

            var files = ServiceLocator.FileSearch.GetModels(Organization.Identifier, CredentialIdentifier, null, false);

            if (files.Any())
            {
                var file = files.First();

                var url = storage.GetFileUrl(file.FileIdentifier, file.FileName, true);

                AchievementFile.Text = $"<a href='{url}'>{file.FileName}</a>";

                FileIdentifier = file.FileIdentifier;
            }
            else
            {
                DeleteAttachment.Visible = false;
            }
        }

        private string GetSearchUrl()
            => "/ui/portal/record/credentials/learners/search";

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect(GetSearchUrl());
    }
}