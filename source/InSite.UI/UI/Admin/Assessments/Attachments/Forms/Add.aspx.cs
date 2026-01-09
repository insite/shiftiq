using System;
using System.IO;

using InSite.Admin.Assessments.Attachments.Controls;
using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Path = System.IO.Path;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/assessments/banks/search";
        private const string SelfUrl = "/ui/admin/assessments/attachments/add";
        private const string ParentUrl = "/ui/admin/assessments/banks/outline";

        #endregion

        #region Properties

        private Guid? BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : (Guid?)null;

        private Guid? StorageID => Guid.TryParse(Request.QueryString["storage"], out var value) ? value : (Guid?)null;

        private int? AssetNumber
        {
            get => (int?)ViewState[nameof(AssetNumber)];
            set => ViewState[nameof(AssetNumber)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileUploadedButton.Click += FileUploadedButton_Click;

            AttachButton.Click += AttachButton_Click;
            CancelButton1.Click += CancelButton_Click;
            CancelButton2.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            LoadBank();
        }

        private void LoadBank()
        {
            var bank = BankID.HasValue ? ServiceLocator.BankSearch.GetBankState(BankID.Value) : null;
            if (bank == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            AssetNumber = bank.Asset;

            Details.Setup(bank.Identifier);

            if (StorageID.HasValue)
                LoadStorage(StorageID.Value);
        }

        private void LoadStorage(Guid storageId)
        {
            var attachment = AttachmentHelper.LoadAttachmentInfo(storageId);
            if (attachment == null || !AttachmentHelper.ExistAttachmentData(storageId))
            {
                ScreenStatus.AddMessage(AlertType.Error, "File not found.");
                return;
            }

            AttachTab.Visible = true;
            AttachTab.IsSelected = true;

            Details.SetInputValues(attachment);

            if (attachment.Image != null)
            {
                var img = attachment.Image;

                Details.SetupResolution(img.Resolution, false);
                Details.SetupActualDimension(img.Actual, false);
            }

            var isImage = attachment.Type == AttachmentType.Image;

            ThumbnailImageColumn.Visible = isImage;

            if (!isImage)
                return;

            Thumbnail.TryLoadData(new AttachmentImageThumbnail.ThumbnailInfo
            {
                FileName = attachment.File.Name + attachment.File.Extension,
                ImageDimension = attachment.Image?.Actual,
                ReadFile = read => AttachmentHelper.ReadAttachmentData(storageId, read),
                GetImageUrl = () =>
                {
                    var type = MimeMapping.GetContentType(attachment.File.Name);
                    var data = AttachmentHelper.LoadAttachmentData(storageId);

                    return $"data:{type};base64,{Convert.ToBase64String(data)}";
                }
            });
        }

        #endregion

        #region Event handlers

        private void FileUploadedButton_Click(object sender, EventArgs e)
        {
            if (!FileInput.HasFile)
                return;

            if (StorageID.HasValue)
                AttachmentHelper.DeleteStorage(StorageID.Value);

            var fileInfo = new FileInfo(FileInput.Metadata.FilePath);

            if (FileExtension.IsImage(FileInput.Metadata.FilePath))
            {
                var limit = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
                if (fileInfo.Length > limit)
                {
                    var ex = new FileStorage.MaxFileSizeExceededException("image", fileInfo.Name, fileInfo.Length, limit);
                    ShowError(ex.Message);
                    return;
                }
            }
            else
            {
                var limit = Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize;
                if (fileInfo.Length > limit)
                {
                    var ex = FileStorage.MaxFileSizeExceededException.Create("document", fileInfo.Name, fileInfo.Length, limit);
                    ShowError(ex.Message);
                    return;
                }
            }

            try
            {
                var storageId = AttachmentHelper.SaveTempFile(fileInfo);

                HttpResponseHelper.Redirect($"{SelfUrl}?bank={BankID}&storage={storageId}");
            }
            catch (ApplicationError appex)
            {
                ShowError(appex.Message);
            }

            void ShowError(string message)
            {
                ScreenStatus.AddMessage(AlertType.Error, message);
                FileInput.ClearMetadata();
                AttachTab.Visible = false;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (StorageID.HasValue)
                AttachmentHelper.DeleteStorage(StorageID.Value);

            HttpResponseHelper.Redirect($"{ParentUrl}?bank={BankID}&panel=attachments");
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Attachment attachment;

            try
            {
                attachment = AttachCurrentFile(StorageID.Value);
                if (attachment == null)
                    return;
            }
            catch (ApplicationError appex)
            {
                AppSentry.SentryError(appex);

                ScreenStatus.AddMessage(AlertType.Error, appex.Message);

                return;
            }

            AttachmentHelper.DeleteStorage(StorageID.Value);

            HttpResponseHelper.Redirect($"{ParentUrl}?bank={BankID}&attachment={attachment.Identifier}");
        }

        #endregion

        #region Methods (attachment)

        private Attachment AttachCurrentFile(Guid storageId)
        {
            var initialAttachment = AttachmentHelper.LoadAttachmentInfo(storageId);
            if (initialAttachment == null || !AttachmentHelper.ExistAttachmentData(storageId))
                HttpResponseHelper.Redirect($"{SelfUrl}?bank={BankID}&storage={storageId}");

            var actualAttachment = Details.GetAttachmentInfo();
            var dbAttachment = new Attachment
            {
                Identifier = UniqueIdentifier.Create(),
                Author = initialAttachment.Author,
                Type = initialAttachment.Type,
                Condition = actualAttachment.Condition,
            };
            dbAttachment.Content.Title.Default = actualAttachment.Title;

            byte[] uploadData;

            if (dbAttachment.Type == AttachmentType.Image)
            {
                dbAttachment.Image = new AttachmentImage
                {
                    Resolution = actualAttachment.Image.Resolution,
                    IsColor = actualAttachment.Image.IsColor,
                    Actual = new ImageDimension
                    {
                        Width = actualAttachment.Image.Actual.Width,
                        Height = actualAttachment.Image.Actual.Height
                    },
                    TargetOnline = new ImageDimension
                    {
                        Width = actualAttachment.Image.TargetOnline.Width,
                        Height = actualAttachment.Image.TargetOnline.Height
                    },
                    TargetPaper = new ImageDimension
                    {
                        Width = actualAttachment.Image.TargetPaper.Width,
                        Height = actualAttachment.Image.TargetPaper.Height
                    }
                };

                if (dbAttachment.Image.Actual.Width != initialAttachment.Image.Actual.Width || dbAttachment.Image.Actual.Height != initialAttachment.Image.Actual.Height)
                {
                    initialAttachment.Image.Actual.Width = dbAttachment.Image.Actual.Width;
                    initialAttachment.Image.Actual.Height = dbAttachment.Image.Actual.Height;

                    uploadData = AttachmentHelper.LoadAndResizeImageData(storageId, initialAttachment);
                }
                else
                {
                    uploadData = AttachmentHelper.LoadAttachmentData(storageId);
                }
            }
            else
            {
                uploadData = AttachmentHelper.LoadAttachmentData(storageId);
            }

            var name = Path.GetFileNameWithoutExtension(actualAttachment.File.Name);
            var extension = Path.GetExtension(actualAttachment.File.Name);
            var filename = StringHelper.ToIdentifier(name) + extension;

            var filePath = AttachmentHelper.GetUniqueFilePath(AssetNumber.Value, filename, initialAttachment.File.Extension);
            var file = FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, uploadData);

            dbAttachment.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            dbAttachment.Upload = file.Guid;

            ServiceLocator.SendCommand(new AddAttachment(BankID.Value, dbAttachment));

            return dbAttachment;
        }

        public static Attachment AttachFile(Guid bankId, int assetNumber, string filename, string title, Stream inputStream)
        {
            var filePath = AttachmentHelper.GetUniqueFilePath(assetNumber, filename);
            var file = FileHelper.Provider.Save(Organization.OrganizationIdentifier, filePath, inputStream);

            return AttachFile(bankId, file, title);
        }

        public static Attachment AttachFile(Guid bankId, FileModel file, string title)
        {
            var attachment = new Attachment
            {
                Identifier = UniqueIdentifier.Create(),
                Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset),
                Author = User.UserIdentifier,
                Type = Attachment.GetAttachmentType(file.Type),
                Upload = file.Guid
            };

            attachment.Content.Title.Default = title.IfNullOrEmpty(file.Name);

            if (attachment.Type == AttachmentType.Image)
            {
                using (var fileStream = FileHelper.Provider.Read(file.OrganizationIdentifier, file.Path))
                    attachment.Image = AttachmentHelper.ReadImageProps(fileStream);
            }

            ServiceLocator.SendCommand(new AddAttachment(bankId, attachment));

            return attachment;
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