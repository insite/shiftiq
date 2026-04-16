using System;
using System.IO;
using System.Web.UI;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Application.Banks.Write;
using InSite.Application.Files.Read;
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
            public Guid? FileId { get; set; }
            public Guid UploadId { get; set; }
            public Guid? StorageId { get; set; }
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

            if (CurrentData.StorageId.HasValue)
                AttachmentHelper.DeleteStorage(CurrentData.StorageId.Value);

            try
            {
                var fileInfo = new FileInfo(FileInput.Metadata.FilePath);

                CurrentData.StorageId = AttachmentHelper.SaveTempFile(fileInfo);
            }
            catch (ApplicationError appex)
            {
                CurrentData.StorageId = null;
                ScreenStatus.AddMessage(AlertType.Error, appex.Message);
            }

            SetupImageReplacement();
        }

        private void RemoveImageReplacementButton_Click(object sender, EventArgs e)
        {
            if (CurrentData.StorageId.HasValue)
                AttachmentHelper.DeleteStorage(CurrentData.StorageId.Value);

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

            if (CurrentData.Type == AttachmentType.Image && CurrentData.StorageId.HasValue)
            {
                var fileInfo = AttachmentHelper.LoadAttachmentInfo(CurrentData.StorageId.Value);
                if (fileInfo != null)
                {
                    var (fileId, uploadId) = SaveImage(attachment, fileInfo);

                    ServiceLocator.SendCommand(new ChangeAttachmentImage(BankID, AttachmentID, fileId, uploadId, User.UserIdentifier, attachment.Image.Actual));
                }
            }
            else
                ModifyFileName(attachment);

            var content = new ContentTitle();
            content.Title.Default = attachment.Title;

            ServiceLocator.SendCommand(new ChangeAttachment(BankID, AttachmentID, attachment.Condition, content, attachment.Image));
        }

        private (Guid? fileId, Guid uploadId) SaveImage(AttachmentInfo attachment, AttachmentInfo fileInfo)
        {
            byte[] uploadData;

            if (attachment.Image.Actual.Width != fileInfo.Image.Actual.Width || attachment.Image.Actual.Height != fileInfo.Image.Actual.Height)
            {
                fileInfo.Image.Actual.Width = attachment.Image.Actual.Width;
                fileInfo.Image.Actual.Height = attachment.Image.Actual.Height;

                uploadData = AttachmentHelper.LoadAndResizeImageData(CurrentData.StorageId.Value, fileInfo);
            }
            else
            {
                uploadData = AttachmentHelper.LoadAttachmentData(CurrentData.StorageId.Value);
            }

            var name = Path.GetFileNameWithoutExtension(fileInfo.File.Name);
            var extension = Path.GetExtension(fileInfo.File.Name);
            var filename = StringHelper.ToIdentifier(name) + extension;

            if (CurrentData.FileId == null)
            {
                var filePath = AttachmentHelper.GetUniqueFilePath(CurrentData.BankAsset, filename, fileInfo.File.Extension);
                return (null, FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, uploadData).Guid);
            }

            FileStorageModel model;

            using (var file = new MemoryStream(uploadData))
            {
                model = ServiceLocator.StorageService.Create(
                    file,
                    filename,
                    Organization.OrganizationIdentifier,
                    User.Identifier,
                    BankID,
                    FileObjectType.Bank,
                    new FileProperties { DocumentName = filename },
                    null
                );
            }

            return (model.FileIdentifier, Guid.Empty);
        }

        private void ModifyFileName(AttachmentInfo attachment)
        {
            if (CurrentData.FileId.HasValue)
            {
                var model = ServiceLocator.StorageService.GetFile(CurrentData.FileId.Value);
                var newFileName = ServiceLocator.StorageService.AdjustFileName(attachment.File.Name + model.GetExtension());
                if (!string.Equals(model.FileName, newFileName))
                    ServiceLocator.StorageService.RenameFile(CurrentData.FileId.Value, User.Identifier, newFileName);

                return;
            }

            FileHelper.Provider.Update(CurrentData.UploadId, file =>
            {
                file.Name = StringHelper.ToIdentifier(attachment.File.Name) + file.Type;

                for (var j = 1; ; j++)
                {
                    if (!UploadSearch.ExistsByOrganizationIdentifier(file.OrganizationIdentifier, CurrentData.UploadId, file.Path))
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
                FileId = attachment.FileIdentifier,
                UploadId = attachment.Upload,
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

            SetFileInfo(info);

            Details.SetInputValues(info);
            Details.SetupAsset(bank, attachment);

            SetupImageReplacement();

            CancelButton.NavigateUrl = GetReaderUrl(AttachmentID);
        }

        private void SetFileInfo(AttachmentInfo attachment)
        {
            if (CurrentData.FileId.HasValue)
            {
                var model = ServiceLocator.StorageService.GetFile(CurrentData.FileId.Value);
                if (model != null)
                {
                    CurrentData.FileName = model.FileName;
                    CurrentData.NavigateUrl = ServiceLocator.StorageService.GetFileUrl(model);

                    attachment.File = new AttachmentFileInfo
                    {
                        Name = Path.GetFileNameWithoutExtension(model.FileName),
                        Extension = model.GetExtension(),
                        ContentLength = model.FileSize,
                    };
                }
            }
            else
            {
                var file = UploadSearch.Select(CurrentData.UploadId);
                if (file != null)
                {
                    CurrentData.FileName = file.Name;
                    CurrentData.NavigateUrl = file.NavigateUrl;

                    attachment.File = new AttachmentFileInfo
                    {
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                        Extension = file.ContentType,
                        ContentLength = file.ContentSize ?? 0,
                    };
                }
            }

            if (attachment.File == null)
            {
                ScreenStatus.AddMessage(AlertType.Error, "File not found.");
                SaveButton.Visible = false;
            }
        }

        private void SetupImageReplacement()
        {
            var isImage = CurrentData.Type == AttachmentType.Image;

            ImageColumn.Visible = isImage;

            if (!isImage)
                return;

            AttachmentImageThumbnail.ThumbnailInfo thumbnailInfo = null;
            var hasFileReplacement = false;

            if (CurrentData.StorageId.HasValue)
            {
                var attachment = AttachmentHelper.LoadAttachmentInfo(CurrentData.StorageId.Value);

                hasFileReplacement = attachment != null;

                if (hasFileReplacement)
                {
                    thumbnailInfo = new AttachmentImageThumbnail.ThumbnailInfo
                    {
                        FileName = attachment.File.Name + attachment.File.Extension,
                        ImageDimension = attachment.Image?.Actual,
                        ReadFile = read => AttachmentHelper.ReadAttachmentData(CurrentData.StorageId.Value, read),
                        GetImageUrl = () =>
                        {
                            var type = MimeMapping.GetContentType(attachment.File.Name);
                            var data = AttachmentHelper.LoadAttachmentData(CurrentData.StorageId.Value);

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
                thumbnailInfo = CreateThumbnailInfo();

                Details.SetupResolution(CurrentData.ImageResolution, true);
                Details.SetupActualDimension(CurrentData.ImageActualDimension, true);
            }

            Thumbnail.TryLoadData(thumbnailInfo);
        }

        private AttachmentImageThumbnail.ThumbnailInfo CreateThumbnailInfo()
        {
            return new AttachmentImageThumbnail.ThumbnailInfo
            {
                FileName = CurrentData.FileName,
                ImageDimension = CurrentData.ImageActualDimension,
                ReadFile = read =>
                {
                    if (CurrentData.FileId.HasValue)
                    {
                        var (_, stream) = ServiceLocator.StorageService.GetFileStream(CurrentData.FileId.Value);
                        using (stream)
                            read(stream);
                    }
                    else
                    {
                        using (var stream = FileHelper.Provider.Read(CurrentData.OrganizationID, CurrentData.NavigateUrl))
                            read(stream);
                    }
                },
                GetImageUrl = () => CurrentData.FileId == null ? FileHelper.GetUrl(CurrentData.NavigateUrl) : CurrentData.NavigateUrl
            };
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