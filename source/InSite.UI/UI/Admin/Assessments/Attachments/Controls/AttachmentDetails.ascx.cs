using System;
using System.Linq;

using Humanizer;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class AttachmentDetails : BaseUserControl
    {
        #region Properties

        private Guid BankIdentifier
        {
            get => (Guid)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        private Guid? AttachmentIdentifier
        {
            get => (Guid?)ViewState[nameof(AttachmentIdentifier)];
            set => ViewState[nameof(AttachmentIdentifier)] = value;
        }

        protected decimal? ImageAspectRatio
        {
            get => (decimal?)ViewState[nameof(ImageAspectRatio)];
            set => ViewState[nameof(ImageAspectRatio)] = value;
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssetVersionIncrement.Click += OnAssetVersionIncremented;
        }

        public void Setup(Guid bankId) => SetupInternal(bankId, null, false);

        public void Setup(Guid bankId, Guid attachmentId, bool isNewVersion) => SetupInternal(bankId, attachmentId, isNewVersion);

        private void SetupInternal(Guid bankId, Guid? attachmentId, bool isNewVersion = false)
        {
            BankIdentifier = bankId;
            AttachmentIdentifier = attachmentId;

            AssetVersionIncremented.Visible = isNewVersion;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            AttachmentTitleValidator.ValidationGroup = groupName;
            AttachmentConditionRequiredValidator.ValidationGroup = groupName;
            FileNameRequiredValidator.ValidationGroup = groupName;
            FileNamePatternValidator.ValidationGroup = groupName;
            ImageActualDimensionWidthValidator.ValidationGroup = groupName;
            ImageActualDimensionHeightValidator.ValidationGroup = groupName;
        }

        #endregion

        #region Event handlers

        private void OnAssetVersionIncremented(object sender, EventArgs e) => Upgrade();

        private void Upgrade()
        {
            var attachmentId = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new UpgradeAttachment(
                BankIdentifier,
                AttachmentIdentifier.Value,
                attachmentId));

            var url = HttpRequestHelper.GetCurrentWebUrl();
            url.QueryString["attachment"] = attachmentId.ToString();
            url.QueryString["new"] = bool.TrueString;

            HttpResponseHelper.Redirect(url);
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(AttachmentInfo attachment)
        {
            var author = UserSearch.Bind(attachment.Author, x => new { x.FullName });

            AttachmentTitle.Text = attachment.Title;
            AttachmentCondition.Value = attachment.Condition;
            AttachmentAuthorName.Text = author == null ? "(Unknown)" : author.FullName;
            AttachmentPublicationStatus.Text = attachment.PublicationStatus.GetDescription();
            AttachmentUploadedDate.Text = attachment.Uploaded.Format(User.TimeZone);

            if (attachment.File != null)
            {
                FileName.Text = attachment.File.Name;
                FileExtension.Text = attachment.File.Extension;
                FileSize.Text = attachment.File.ContentLength.Bytes().Humanize("0.##");
            }
            else
            {
                FileNameField.Visible = false;
                FileExtensionField.Visible = false;
                FileSizeField.Visible = false;
                AttachmentPublicationStatusColumn.Visible = false;
            }

            if (attachment.Type == AttachmentType.Image)
            {
                var attachmentImage = attachment.Image;

                AttachmentImageFieldsContainer.Visible = true;

                ImageOnlineDimensionWidth.ValueAsInt = Number.NullIfOutOfRange(attachmentImage.TargetOnline?.Width, 1, attachmentImage.Actual.Width);
                ImageOnlineDimensionHeight.ValueAsInt = Number.NullIfOutOfRange(attachmentImage.TargetOnline?.Height, 1, attachmentImage.Actual.Height);
                ImagePaperDimensionWidth.ValueAsInt = Number.NullIfOutOfRange(attachmentImage.TargetPaper?.Width, 1, attachmentImage.Actual.Width);
                ImagePaperDimensionHeight.ValueAsInt = Number.NullIfOutOfRange(attachmentImage.TargetPaper?.Height, 1, attachmentImage.Actual.Height);

                ImageIsColor.ValueAsBoolean = attachmentImage.IsColor;
            }
        }

        internal void SetupResolution(int value, bool readOnly)
        {
            ImageResolutionOutput.Text = value.ToString("n0");
            ImageResolutionInput.ValueAsInt = value;
            ImageResolutionInput.MaxValue = int.MaxValue;

            ImageResolutionOutputField.Visible = readOnly;
            ImageResolutionInputField.Visible = !readOnly;
        }

        internal void SetupActualDimension(ImageDimension value, bool readOnly)
        {
            SetMaxValue(int.MaxValue, int.MaxValue);

            ImageActualDimensionWidthOutput.Text = value.Width.ToString("n0");
            ImageActualDimensionHeightOutput.Text = value.Height.ToString("n0");

            SetValue(ImageActualDimensionWidthInput, value.Width);
            SetValue(ImageActualDimensionHeightInput, value.Height);

            ImageActualDimensionOutputField.Visible = readOnly;
            ImageActualDimensionInputField.Visible = !readOnly;

            SetMaxValue(value.Width, value.Height);

            ImageAspectRatio = (decimal)value.Width / value.Height;

            void SetMaxValue(int maxWidth, int maxHeight)
            {
                SetGroupMaxValue(ImageActualDimensionWidthInput, ImageActualDimensionHeightInput, maxWidth, maxHeight);
                SetGroupMaxValue(ImageOnlineDimensionWidth, ImageOnlineDimensionHeight, maxWidth, maxHeight);
                SetGroupMaxValue(ImagePaperDimensionWidth, ImagePaperDimensionHeight, maxWidth, maxHeight);
            }

            void SetGroupMaxValue(NumericBox ctrlWidth, NumericBox ctrlHeight, int maxWidth, int maxHeight)
            {
                var width = ctrlWidth.ValueAsInt;
                var height = ctrlHeight.ValueAsInt;

                if (width.HasValue && height.HasValue && (width.Value > maxWidth || height.Value > maxHeight))
                {
                    ctrlWidth.ValueAsInt = maxWidth;
                    ctrlHeight.ValueAsInt = maxHeight;
                }

                ctrlWidth.MaxValue = maxWidth;
                ctrlHeight.MaxValue = maxHeight;
            }

            void SetValue(NumericBox ctrl, int ctrlValue)
            {
                ctrl.ValueAsInt = Number.CheckRange(ctrlValue, (int)ctrl.MinValue, (int)ctrl.MaxValue);
            }
        }

        internal void SetupAsset(BankState bank, Attachment attachment)
        {
            AssetRow.Visible = true;

            AssetNumber.Text = attachment.Asset.ToString();
            AssetVersion.Text = attachment.AssetVersion.ToString();

            var versions = attachment
                .EnumerateAllVersions(SortOrder.Descending)
                .Where(x => x != attachment)
                .Select(x => new
                {
                    Number = x.AssetVersion,
                    NavigateUrl = $"/ui/admin/assessments/attachments/change?bank={bank.Identifier}&attachment={x.Identifier}"
                })
                .ToArray();

            AssetVersionRepeater.DataSource = versions;
            AssetVersionRepeater.DataBind();
            AssetVersionIncrement.Visible = versions.Length == 0 || attachment.IsLastVersion();

            var changes = AttachmentHelper.GetChanges(bank.Identifier, attachment.Identifier);
            var returnUrl = Request.QueryString["return"].IsNotEmpty()
                ? new ReturnUrl($"bank&attachment&return")
                : new ReturnUrl($"bank&attachment");

            ChangesCount.Text = "event".ToQuantity(changes.Count());
            ChangesCountField.Visible = true;
            ViewChangesLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/attachments/history?bank={bank.Identifier}&attachment={attachment.Identifier}");

            UsageCount.Text = "question".ToQuantity(attachment.QuestionIdentifiers.Count);
            UsageCountField.Visible = true;
            ViewUsageLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/attachments/usage?bank={bank.Identifier}&attachment={attachment.Identifier}");
        }

        public void GetInputValues(AttachmentInfo attachment)
        {
            attachment.Title = AttachmentTitle.Text;
            attachment.Condition = AttachmentCondition.Value.NullIfEmpty();

            if (FileNameField.Visible)
            {
                if (attachment.File == null)
                    attachment.File = new AttachmentFileInfo();

                attachment.File.Name = FileName.Text;
            }

            if (AttachmentImageFieldsContainer.Visible)
            {
                var attachmentImage = attachment.Image ?? (attachment.Image = new AttachmentImage());

                if (attachmentImage.Actual == null)
                    attachmentImage.Actual = new ImageDimension();

                if (attachmentImage.TargetOnline == null)
                    attachmentImage.TargetOnline = new ImageDimension();

                if (attachmentImage.TargetPaper == null)
                    attachmentImage.TargetPaper = new ImageDimension();

                attachmentImage.Resolution = ImageResolutionInput.ValueAsInt ?? 0;
                attachmentImage.Actual.Width = ImageActualDimensionWidthInput.ValueAsInt.Value;
                attachmentImage.Actual.Height = ImageActualDimensionHeightInput.ValueAsInt.Value;
                attachmentImage.TargetOnline.Width = ImageOnlineDimensionWidth.ValueAsInt ?? 0;
                attachmentImage.TargetOnline.Height = ImageOnlineDimensionHeight.ValueAsInt ?? 0;
                attachmentImage.TargetPaper.Width = ImagePaperDimensionWidth.ValueAsInt ?? 0;
                attachmentImage.TargetPaper.Height = ImagePaperDimensionHeight.ValueAsInt ?? 0;
                attachmentImage.IsColor = ImageIsColor.ValueAsBoolean.Value;
            }
        }

        public AttachmentInfo GetAttachmentInfo()
        {
            var result = new AttachmentInfo();

            GetInputValues(result);

            return result;
        }

        #endregion
    }
}