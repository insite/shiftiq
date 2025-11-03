using System;

using Humanizer;

using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.People.Forms
{
    public partial class DeleteAttachment : AdminBasePage, IHasParentLinkParameters
    {
        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["contact"], out var value) ? value : Guid.Empty;

        private Guid FileIdentifier => Guid.TryParse(Request.QueryString["file"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileName.Click += FileName_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(PermissionIdentifiers.Admin_Contacts, PermissionOperation.Delete))
                RedirectToSearch();

            if (!IsPostBack)
                LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.StorageService.Delete(FileIdentifier);

            RedirectToUser();
        }

        private void FileName_Click(object sender, EventArgs e)
        {
            var (status, model, stream) = ServiceLocator.StorageService.GetFileStreamAndAuthorize(Identity, FileIdentifier);

            if (status != FileGrantStatus.Granted)
                RedirectToSearch();
            else
                Response.SendFile(model.FileName, stream, model.FileSize, "application/octet-stream");
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/contacts/people/search", true);

        private void RedirectToUser() => HttpResponseHelper.Redirect(GetUserUrl(), true);

        private string GetUserUrl()
        {
            return $"/ui/admin/contacts/people/edit?contact={UserIdentifier}&panel=attachments";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit") ? $"contact={UserIdentifier}&panel=attachments" : null;
        }

        private void LoadData()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.User);
            if (person == null)
            {
                RedirectToSearch();
                return;
            }

            var (status, file) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, FileIdentifier);
            if (status != FileGrantStatus.Granted || file.ObjectIdentifier != UserIdentifier)
            {
                RedirectToUser();
                return;
            }

            PageHelper.AutoBindHeader(this, null, person.User.FullName);

            Timestamp.Text = $"Posted on {file.Uploaded.Format(User.TimeZone)}";
            FileName.Text = file.Properties.DocumentName;
            FileSize.Text = file.FileSize.Bytes().Humanize("0.##");

            CancelButton.NavigateUrl = GetUserUrl();
        }
    }
}