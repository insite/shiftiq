using System;
using System.IO;
using System.Web.UI.WebControls;

using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;
using Path = System.IO.Path;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class EditUploads : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        protected string GetUploadUrl(object typeObj, object guidObj, object nameObj)
        {
            return ((string)typeObj) == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl((Guid)guidObj, (string)nameObj)
                : (string)nameObj;
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (!Access.Read)
                return;

            if (Access.Administrate || Access.Configure)
                return;

            var info = VCmdsAchievementSearch.Select(AchievementIdentifier.Value);
            if (info.Visibility != AccountScopes.Organization || info.OrganizationIdentifier != CurrentIdentityFactory.ActiveOrganizationIdentifier)
                Access = Access.SetAll(false);
        }

        private Guid? AchievementIdentifier => Guid.TryParse(Request.QueryString["id"], out Guid n) ? n : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadButton.Click += UploadButton_Click;

            NewFile.AutoPostBack = true;
            NewFile.CheckedChanged += (x, y) => ShowFields();

            NewLink.AutoPostBack = true;
            NewLink.CheckedChanged += (x, y) => ShowFields();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var entity = AchievementIdentifier.HasValue ? VCmdsAchievementSearch.Select(AchievementIdentifier.Value) : null;
            if (entity == null)
                HttpResponseHelper.Redirect("/ui/cmds/admin/achievements/search", true);

            PageHelper.AutoBindHeader(this, null, entity.AchievementTitle);

            ShowFields();

            BindAttachments();
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var isSaved = NewFile.Checked
                ? SaveFiles()
                : SaveLink();

            if (!isSaved)
                return;

            ScreenStatus.AddMessage(AlertType.Success, "Attachment(s) has been uploaded.");

            BindAttachments();
        }

        protected void Attachment_Command(object sender, CommandEventArgs e)
        {
            if (StringHelper.Equals("DeleteFile", e.CommandName))
            {
                var uploadId = Guid.Parse(e.CommandArgument.ToString());

                var entity = UploadSearch.Select(uploadId);

                UploadRepository.DeleteRelations(uploadId);

                if (entity != null)
                {
                    if (entity.UploadType == UploadType.CmdsFile)
                        CmdsUploadProvider.Current.Delete(entity.ContainerIdentifier, entity.Name);
                    else
                        UploadStore.DeleteLink(uploadId);

                    ScreenStatus.AddMessage(AlertType.Success, "Attachment has been removed.");
                }
                else
                {
                    ScreenStatus.AddMessage(AlertType.Warning, "Attachment not found.");
                }

                BindAttachments();
            }
        }

        private void BindAttachments()
        {
            var uploadsCount = UploadRepository.CountAllAchievementUploads(AchievementIdentifier.Value);
            AttachmentsCount.Text = uploadsCount.ToString();

            Attachments.DataSource = UploadRepository.SelectAllAchievementUploads(AchievementIdentifier.Value, Paging.SetSkipTake(0, 1000));
            Attachments.DataBind();
        }

        private bool SaveLink()
        {
            if (UploadSearch.Exists(AchievementIdentifier.Value, NavigationUrl.Text))
            {
                ScreenStatus.AddMessage(AlertType.Error, "The link already exists.");
                return false;
            }

            var entity = VCmdsAchievementSearch.Select(AchievementIdentifier.Value);

            UploadStore.InsertLink(new Upload
            {
                OrganizationIdentifier = entity.OrganizationIdentifier,
                ContainerIdentifier = entity.AchievementIdentifier,
                ContainerType = UploadContainerType.Asset,

                Name = NavigationUrl.Text,
                Title = UrlTitle.Text,

                Uploader = User.UserIdentifier,
                Uploaded = DateTimeOffset.UtcNow
            });

            NavigationUrl.Text = null;
            UrlTitle.Text = null;

            return true;
        }

        private bool SaveFiles()
        {
            var files = UploadHelper.GetFiles(FileUpload.Metadata);
            if (files.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "No files to upload");
                return false;
            }

            foreach (var source in files)
            {
                var filename = Path.GetFileName(source);

                try
                {
                    using (var stream = new FileStream(source, FileMode.Open))
                        CmdsUploadProvider.Current.Save(UploadContainerType.Asset, AchievementIdentifier.Value, filename, stream);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("File is empty"))
                        throw;
                }
            }

            return true;
        }

        private void ShowFields()
        {
            FileField.Visible = NewFile.Checked;
            LinkField1.Visible = NewLink.Checked;
            LinkField2.Visible = NewLink.Checked;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"id={AchievementIdentifier}" : null;
    }
}
