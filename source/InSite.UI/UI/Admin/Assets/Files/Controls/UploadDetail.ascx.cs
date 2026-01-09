using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assets.Files.Controls
{
    public partial class UploadDetail : BaseUserControl
    {
        public event StringValueHandler UploadError;

        private Guid ObjectIdentifier
        {
            get => (Guid)ViewState[nameof(ObjectIdentifier)];
            set => ViewState[nameof(ObjectIdentifier)] = value;
        }

        private FileObjectType ObjectType
        {
            get => (FileObjectType)ViewState[nameof(ObjectType)];
            set => ViewState[nameof(ObjectType)] = value;
        }

        private string OriginalDocumentName
        {
            get => (string)ViewState[nameof(OriginalDocumentName)];
            set => ViewState[nameof(OriginalDocumentName)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileCategory.AutoPostBack = true;
            FileCategory.ValueChanged += FileCategory_ValueChanged;

            IssueDocumentNameValidator.ServerValidate += IssueDocumentNameValidator_ServerValidate;

            File.FileUploaded += File_FileUploaded;
        }

        private void IssueDocumentNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ObjectType != FileObjectType.Issue
                || string.Equals(DocumentName.Text, OriginalDocumentName)
                || ServiceLocator.IssueSearch.GetAttachment(ObjectIdentifier, DocumentName.Text) == null;
        }

        private void FileCategory_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            FileSubcategory.ClearSelection();
            FileSubcategory.DocumentType = FileCategory.Value;
            FileSubcategory.RefreshData();
        }

        private void File_FileUploaded(object sender, EventArgs e)
        {
            if (ObjectType != FileObjectType.User)
                return;

            var error = ValidateUserFile(ObjectIdentifier);

            if (!string.IsNullOrEmpty(error))
            {
                File.DeleteFile();
                UploadError?.Invoke(this, new StringValueArgs(error));
            }
        }

        public (bool IsValid, string Title) BindDefaultsToControls(FileObjectType objectType, Guid objectIdentifier)
        {
            var (isValid, title) = Validate(objectType, objectIdentifier);
            if (!isValid)
                return (isValid, title);

            ObjectIdentifier = objectIdentifier;
            ObjectType = objectType;

            PermissionList.BindDefaultsToControls();

            FileStatus.EnsureDataBound();
            FileStatus.Value = "Uploaded";

            if (objectType != FileObjectType.Issue  || CaseAttachmentHelper.AllowLearnerToViewByIssue(ObjectIdentifier))
                AllowLearnerToViewYes.Checked = true;
            else
                AllowLearnerToViewNo.Checked = true;

            return (isValid, title);
        }

        public (bool IsValid, string Title) BindModelToControls(FileStorageModel model)
        {
            var (isValid, title) = Validate(model.ObjectType, model.ObjectIdentifier);
            if (!isValid)
                return (isValid, title);

            ObjectIdentifier = model.ObjectIdentifier;
            ObjectType = model.ObjectType;
            OriginalDocumentName = model.Properties.DocumentName;

            PermissionList.BindModelToControls(model);

            UploadField.Visible = false;
            LinkField.Visible = true;

            FileLink.HRef = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false);
            FileName.Text = model.FileName;
            FileSize.Text = model.FileSize.Bytes().Humanize("#");

            var properties = model.Properties;

            FileStatus.EnsureDataBound();
            FileStatus.Value = properties.Status;

            FileCategory.EnsureDataBound();
            FileCategory.Value = properties.Category;

            FileSubcategory.DocumentType = properties.Category;
            FileSubcategory.RefreshData();
            FileSubcategory.Value = properties.Subcategory;

            FileDescription.Text = properties.Description;
            DocumentName.Text = properties.DocumentName;
            FileExpiry.Value = properties.Expiry?.UtcDateTime;
            FileReceived.Value = properties.Received?.UtcDateTime;
            FileAlternated.Value = properties.Alternated?.UtcDateTime;
            IsReviewed.Checked = properties.ReviewedTime.HasValue;
            IsApproved.Checked = properties.ApprovedTime.HasValue;

            if (model.Properties.AllowLearnerToView)
                AllowLearnerToViewYes.Checked = true;
            else
                AllowLearnerToViewNo.Checked = true;

            return (isValid, title);
        }

        public FileStorageModel CreateFile(Guid objectIdentifier, FileObjectType objectType)
        {
            var properties = GetProperties(null);
            var claims = PermissionList.GetFileClaims();

            var model = File.SaveFile(objectIdentifier, objectType);

            ServiceLocator.StorageService.ChangeProperties(model.FileIdentifier, User.Identifier, properties, false);
            ServiceLocator.StorageService.ChangeClaims(model.FileIdentifier, claims);

            return model;
        }

        public void UpdateFile(Guid fileIdentifier)
        {
            var file = ServiceLocator.StorageService.GetFile(fileIdentifier);
            var properties = GetProperties(file.Properties);
            var claims = PermissionList.GetFileClaims();

            if (file.ObjectType == FileObjectType.Issue)
            {
                var attachment = ServiceLocator.IssueSearch.GetAttachment(file.ObjectIdentifier, file.FileName);
                var commands = new List<ICommand>();

                if (!string.Equals(file.Properties.DocumentName, properties.DocumentName))
                    commands.Add(new RenameAttachmentFile(file.ObjectIdentifier, file.Properties.DocumentName, properties.DocumentName));

                ServiceLocator.SendCommands(commands);
            }

            ServiceLocator.StorageService.ChangeProperties(fileIdentifier, User.Identifier, properties, true);
            ServiceLocator.StorageService.ChangeClaims(fileIdentifier, claims);
        }

        private string ValidateUserFile(Guid objectIdentifier)
        {
            var imageLimit = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;
            var docLimit = Organization.PlatformCustomization.UploadSettings.Documents.MaximumFileSize;

            if (FileExtension.IsImage(File.FileName) && File.FileSize > imageLimit)
            {
                var ex = new FileStorage.MaxFileSizeExceededException("image", File.FileName, File.FileSize, imageLimit);
                return ex.Message;
            }
            else if (!FileExtension.IsImage(File.FileName) && File.FileSize > docLimit)
            {
                var ex = new FileStorage.MaxFileSizeExceededException("document", File.FileName, File.FileSize, docLimit);
                return ex.Message;
            }

            var existing = ServiceLocator.StorageService.GetGrantedFiles(Identity, objectIdentifier, File.FileName);
            if (existing.Count > 0)
                return $"The file with the name '{File.FileName}' is already exist";

            return null;
        }

        private FileProperties GetProperties(FileProperties oldProperties)
        {
            var (reviewedTime, reviewedUser) = GetTimeAndUser(oldProperties?.ReviewedTime, oldProperties?.ReviewedUserIdentifier, IsReviewed.Checked);
            var (approvedTime, approvedUser) = GetTimeAndUser(oldProperties?.ApprovedTime, oldProperties?.ApprovedUserIdentifier, IsApproved.Checked);

            return new FileProperties
            {
                DocumentName = DocumentName.Text,
                Description = FileDescription.Text,
                Category = FileCategory.Value,
                Subcategory = FileSubcategory.Value,
                Status = FileStatus.Value,
                Expiry = FileExpiry.Value,
                Received = FileReceived.Value,
                Alternated = FileAlternated.Value,
                ReviewedTime = reviewedTime,
                ReviewedUserIdentifier = reviewedUser,
                ApprovedTime = approvedTime,
                ApprovedUserIdentifier = approvedUser,
                AllowLearnerToView = AllowLearnerToViewYes.Checked
            };
        }

        private static (DateTimeOffset? Time, Guid? User) GetTimeAndUser(DateTimeOffset? oldTime, Guid? oldUser, bool selected)
        {
            if (oldTime.HasValue == selected)
                return (oldTime, oldUser);

            if (selected)
                return (DateTimeOffset.UtcNow, User.Identifier);

            return (null, null);
        }

        private (bool, string) Validate(FileObjectType objectType, Guid objectIdentifier)
        {
            switch (objectType)
            {
                case FileObjectType.User:
                    return ValidateUser(objectIdentifier);
                case FileObjectType.Issue:
                    return ValidateIssue(objectIdentifier);
                case FileObjectType.Response:
                    return ValidateResponse(objectIdentifier);
                default:
                    return (false, null);
            }
        }

        private (bool, string) ValidateUser(Guid objectIdentifier)
        {
            if (!ServiceLocator.PersonSearch.IsPersonExist(objectIdentifier, Organization.Identifier))
                return (false, null);

            File.AllowedExtensions = new[] { ".docx", ".gif", ".jpg", ".jpeg", ".pdf", ".png", ".zip" };

            var user = UserSearch.Select(objectIdentifier);

            return (true, user.FullName);
        }

        private (bool, string) ValidateIssue(Guid objectIdentifier)
        {
            var issue = ServiceLocator.IssueSearch.GetIssue(objectIdentifier);
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
                return (false, null);

            return (true, issue.IssueTitle);
        }

        private (bool, string) ValidateResponse(Guid objectIdentifier)
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(objectIdentifier, x => x.SurveyForm);
            if (response == null || response.OrganizationIdentifier != Organization.Identifier)
                return (false, null);

            return (true, response.SurveyForm.SurveyFormName);
        }
    }
}