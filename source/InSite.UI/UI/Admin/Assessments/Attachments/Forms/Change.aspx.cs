using System;
using System.IO;
using System.Web.UI;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using Path = System.IO.Path;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/assessments/banks/search";
        private const string ParentUrl = "/ui/admin/assessments/banks/outline";

        #endregion

        #region Classes

        [Serializable]
        private class ControlData
        {
            public Guid OrganizationID { get; set; }
            public int BankAsset { get; set; }
            public Guid UploadID { get; set; }
            public Guid? FileID { get; set; }
            public AttachmentType Type { get; set; }
            public string FileName { get; set; }
            public string NavigateUrl { get; set; }
            public int ImageResolution { get; set; }
            public ImageDimension ImageActualDimension { get; set; }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid AttachmentID => Guid.TryParse(Request.QueryString["attachment"], out var value) ? value : Guid.Empty;

        private ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            set => ViewState[nameof(CurrentData)] = value;
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            ImageUploadedButton.Click += ImageUploadedButton_Click;
            RemoveImageReplacementButton.Click += RemoveImageReplacementButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void ImageUploadedButton_Click(object sender, EventArgs e)
        {
            if (!FileInput.HasFile)
                return;

            if (!Page.IsValid)
                return;

            if (CurrentData.FileID.HasValue)
                AttachmentHelper.DeleteStorage(CurrentData.FileID.Value);

            try
            {
                var fileInfo = new FileInfo(FileInput.Metadata.FilePath);

                CurrentData.FileID = AttachmentHelper.SaveTempFile(fileInfo);
            }
            catch (ApplicationError appex)
            {
                CurrentData.FileID = null;
                ScreenStatus.AddMessage(AlertType.Error, appex.Message);
            }

            SetupImageReplacement();
        }

        private void RemoveImageReplacementButton_Click(object sender, EventArgs e)
        {
            if (CurrentData.FileID.HasValue)
                AttachmentHelper.DeleteStorage(CurrentData.FileID.Value);

            SetupImageReplacement();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(AttachmentID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var attachment = bank.FindAttachment(AttachmentID);
            if (attachment == null)
                RedirectToReader();

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            Details.Setup(bank.Identifier, attachment.Identifier, Request.QueryString["new"] == bool.TrueString);

            SetInputValues(bank, attachment);
        }

        private void Save()
        {
            var attachment = Details.GetAttachmentInfo();
            if (attachment.File == null)
                return;

            if (CurrentData.Type == AttachmentType.Image && CurrentData.FileID.HasValue)
            {
                var fileInfo = AttachmentHelper.LoadAttachmentInfo(CurrentData.FileID.Value);
                if (fileInfo != null)
                {
                    byte[] uploadData;

                    if (attachment.Image.Actual.Width != fileInfo.Image.Actual.Width || attachment.Image.Actual.Height != fileInfo.Image.Actual.Height)
                    {
                        fileInfo.Image.Actual.Width = attachment.Image.Actual.Width;
                        fileInfo.Image.Actual.Height = attachment.Image.Actual.Height;

                        uploadData = AttachmentHelper.LoadAndResizeImageData(CurrentData.FileID.Value, fileInfo);
                    }
                    else
                    {
                        uploadData = AttachmentHelper.LoadAttachmentData(CurrentData.FileID.Value);
                    }

                    var name = Path.GetFileNameWithoutExtension(fileInfo.File.Name);
                    var extension = Path.GetExtension(fileInfo.File.Name);
                    var filename = StringHelper.ToIdentifier(name) + extension;

                    var filePath = AttachmentHelper.GetUniqueFilePath(CurrentData.BankAsset, filename, fileInfo.File.Extension);
                    var file = FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, uploadData);

                    ServiceLocator.SendCommand(new ChangeAttachmentImage(BankID, AttachmentID, file.Guid, User.UserIdentifier, attachment.Image.Actual));
                }
            }

            var content = new ContentTitle();
            content.Title.Default = attachment.Title;

            ServiceLocator.SendCommand(new ChangeAttachment(BankID, AttachmentID, attachment.Condition, content, attachment.Image));

            FileHelper.Provider.Update(CurrentData.UploadID, file =>
            {
                file.Name = StringHelper.ToIdentifier(attachment.File.Name) + file.Type;

                for (var j = 1; ; j++)
                {
                    if (!UploadSearch.ExistsByOrganizationIdentifier(file.OrganizationIdentifier, CurrentData.UploadID, file.Path))
                        break;

                    file.Name = StringHelper.ToIdentifier($"{attachment.File.Name} ({j})") + file.Type;
                }
            });
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(BankState bank, Attachment attachment)
        {
            CurrentData = new ControlData
            {
                BankAsset = attachment.Asset,
                UploadID = attachment.Upload,
                Type = attachment.Type,
                OrganizationID = bank.Tenant
            };

            if (attachment.Image != null)
            {
                CurrentData.ImageResolution = attachment.Image.Resolution;
                CurrentData.ImageActualDimension = attachment.Image.Actual;
            }

            var info = new AttachmentInfo
            {
                Type = attachment.Type,
                Title = attachment.Content.Title.Default,
                Condition = attachment.Condition,
                PublicationStatus = attachment.PublicationStatus,
                Image = attachment.Image,
                Author = attachment.Author,
                Uploaded = attachment.Uploaded,
            };

            var file = UploadSearch.Select(CurrentData.UploadID);
            if (file != null)
            {
                CurrentData.FileName = file.Name;
                CurrentData.NavigateUrl = file.NavigateUrl;

                info.File = new AttachmentFileInfo
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Extension = file.ContentType,
                    ContentLength = file.ContentSize ?? 0,
                };
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Error, "File not found.");
                SaveButton.Visible = false;
            }

            Details.SetInputValues(info);
            Details.SetupAsset(bank, attachment);

            SetupImageReplacement();

            CancelButton.NavigateUrl = GetReaderUrl(AttachmentID);
        }

        private void SetupImageReplacement()
        {
            var isImage = CurrentData.Type == AttachmentType.Image;

            ImageColumn.Visible = isImage;

            if (!isImage)
                return;

            AttachmentImageThumbnail.ThumbnailInfo thumbnailInfo = null;
            var hasFileReplacement = false;

            if (CurrentData.FileID.HasValue)
            {
                var attachment = AttachmentHelper.LoadAttachmentInfo(CurrentData.FileID.Value);

                hasFileReplacement = attachment != null;

                if (hasFileReplacement)
                {
                    thumbnailInfo = new AttachmentImageThumbnail.ThumbnailInfo
                    {
                        FileName = attachment.File.Name + attachment.File.Extension,
                        ImageDimension = attachment.Image?.Actual,
                        ReadFile = read => AttachmentHelper.ReadAttachmentData(CurrentData.FileID.Value, read),
                        GetImageUrl = () =>
                        {
                            var type = MimeMapping.GetContentType(attachment.File.Name);
                            var data = AttachmentHelper.LoadAttachmentData(CurrentData.FileID.Value);

                            return $"data:{type};base64,{Convert.ToBase64String(data)}";
                        }
                    };

                    ImageReplacementName.Text = thumbnailInfo.FileName;

                    Details.SetupResolution(attachment.Image.Resolution, false);
                    Details.SetupActualDimension(attachment.Image.Actual, false);
                }
            }

            ImageUploadField.Visible = !hasFileReplacement;
            ImageReplacementField.Visible = hasFileReplacement;

            if (thumbnailInfo == null)
            {
                thumbnailInfo = new AttachmentImageThumbnail.ThumbnailInfo
                {
                    FileName = CurrentData.FileName,
                    ImageDimension = CurrentData.ImageActualDimension,
                    ReadFile = read =>
                    {
                        using (var stream = FileHelper.Provider.Read(CurrentData.OrganizationID, CurrentData.NavigateUrl))
                            read(stream);
                    },
                    GetImageUrl = () => FileHelper.GetUrl(CurrentData.NavigateUrl)
                };

                Details.SetupResolution(CurrentData.ImageResolution, true);
                Details.SetupActualDimension(CurrentData.ImageActualDimension, true);
            }

            Thumbnail.TryLoadData(thumbnailInfo);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect(SearchUrl, true);

        private void RedirectToReader(Guid? attachment = null)
        {
            var url = GetReaderUrl(attachment);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? attachment = null)
        {
            var url = new ReturnUrl().GetReturnUrl();

            if (url == null)
            {
                url = ParentUrl + $"?bank={BankID}";
                if (attachment.HasValue)
                    url += $"&attachment={attachment.Value}";
            }

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}