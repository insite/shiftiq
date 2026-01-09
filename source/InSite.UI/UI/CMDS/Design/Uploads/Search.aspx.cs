using System;
using System.IO;
using System.Web.UI;

using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;
using Path = System.IO.Path;

namespace InSite.Cmds.Design.Uploads
{
    public partial class Search : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        #region Properties

        private Guid OrganizationIdentifier => Guid.TryParse(Request.QueryString["id"], out var value)
            ? value : CurrentIdentityFactory.ActiveOrganizationIdentifier;

        private const string SearchUrl = "/ui/cmds/admin/organizations/search";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitButton.Click += SubmitButton_Click;

            SubType.AutoPostBack = true;
            SubType.SelectedIndexChanged += SubType_SelectedIndexChanged;

            Downloads.DataBinding += Downloads_DataBinding;
            Downloads.RowCommand += Downloads_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var organization = OrganizationSearch.Select(OrganizationIdentifier);
            if (organization == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, qualifier: organization.CompanyName);

            LoadDownloads(organization.OrganizationIdentifier);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/edit") ? $"id={OrganizationIdentifier}" : null;

        #endregion

        #region Event handlers

        private void SubmitButton_Click(Object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var organizationId = OrganizationIdentifier;
            var isSaved = SubType.SelectedValue == UploadType.Link
                ? SaveLink(organizationId)
                : SaveFiles(organizationId);

            if (!isSaved)
                return;

            ScreenStatus.AddMessage(AlertType.Success, "Download(s) has been uploaded.");

            LoadDownloads(organizationId);
        }

        private void Downloads_DataBinding(object sender, EventArgs e)
        {
            Downloads.DataSource =
                UploadRepository.SelectWithCounters(
                    OrganizationIdentifier,
                    new[] { UploadType.CmdsFile, UploadType.Link }, Downloads.PageIndex, Downloads.PageSize);
        }

        private void Downloads_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                var grid = (Grid)sender;
                var uploadId = grid.GetDataKey<Guid>(e);

                DeleteFile(uploadId);

                ScreenStatus.AddMessage(AlertType.Success, "Download has been deleted.");

                LoadDownloads(OrganizationIdentifier);
            }
        }

        private void SubType_SelectedIndexChanged(Object sender, EventArgs e)
        {
            FileUpload.ClearMetadata();

            if (SubType.SelectedValue == "File")
                UploadPanels.SetActiveView(FilesPanel);
            else
                UploadPanels.SetActiveView(LinksPanel);
        }

        #endregion

        #region Methods (db operations)

        private void LoadDownloads(Guid organizationId)
        {
            var rowCount = UploadRepository.Count(organizationId, new[] { UploadType.CmdsFile, UploadType.Link });

            Downloads.Visible = rowCount > 0;
            Downloads.VirtualItemCount = rowCount;
            Downloads.DataBind();
        }

        private bool SaveLink(Guid organizationId)
        {
            if (UploadSearch.Exists(organizationId, NavigationUrl.Text))
            {
                ScreenStatus.AddMessage(AlertType.Error, "The link already exists.");
                return false;
            }

            UploadStore.InsertLink(new Upload
            {
                OrganizationIdentifier = organizationId,
                ContainerIdentifier = organizationId,
                ContainerType = UploadContainerType.Oganization,
                UploadPrivacyScope = "Platform",
                Name = NavigationUrl.Text,
                Title = UrlTitle.Text,

                Uploader = User.UserIdentifier,
                Uploaded = DateTimeOffset.UtcNow
            });

            NavigationUrl.Text = null;
            UrlTitle.Text = null;

            return true;
        }

        private bool SaveFiles(Guid organizationId)
        {
            if (!FileUpload.HasFile)
                return false;

            var files = UploadHelper.GetFiles(FileUpload.Metadata);

            foreach (var source in files)
            {
                var filename = Path.GetFileName(source);
                filename = CmdsUploadProvider.AdjustFileName(filename);

                using (var stream = new FileStream(source, FileMode.Open))
                    CmdsUploadProvider.Current.Save(UploadContainerType.Oganization, organizationId, filename, stream);
            }

            return true;
        }

        private void DeleteFile(Guid uploadId)
        {
            var entity = UploadSearch.Select(uploadId);

            UploadRepository.DeleteRelations(uploadId);

            if (entity.UploadType == UploadType.CmdsFile)
                CmdsUploadProvider.Current.Delete(entity.ContainerIdentifier, entity.Name);
            else
                UploadStore.DeleteLink(uploadId);
        }

        #endregion

        #region Methods (helpers)

        protected string GetUploadUrl(object typeObj, object guidObj, object nameObj)
        {
            return (string)typeObj == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl((Guid)guidObj, (string)nameObj)
                : (string)nameObj;
        }

        #endregion
    }
}