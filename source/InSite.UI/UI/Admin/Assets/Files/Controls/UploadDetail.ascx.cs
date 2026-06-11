using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Cases.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Workflow.Cases.Utilities;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
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

        private Guid? CaseId
        {
            get => (Guid?)ViewState[nameof(CaseId)];
            set => ViewState[nameof(CaseId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileCategory.AutoPostBack = true;
            FileCategory.ValueChanged += FileCategory_ValueChanged;

            IssueDocumentNameValidator.ServerValidate += IssueDocumentNameValidator_ServerValidate;

            File.FileUploaded += File_FileUploaded;

            PermissionList.PermissionsChanged += PermissionList_PermissionsChanged;
        }

        private void IssueDocumentNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ObjectType != FileObjectType.Issue
                || string.Equals(DocumentName.Text, OriginalDocumentName)
                || ServiceLocator.IssueSearch.GetAttachment(ObjectIdentifier, DocumentName.Text) == null;
        }

        private void FileCategory_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            FileSubcategorySelectorView.IsActive = true;

            SwitchToManualButton.Enabled = !string.IsNullOrEmpty(FileCategory.Value);

            FileSubcategorySelector.ClearSelection();
            FileSubcategorySelector.DocumentType = FileCategory.Value;
            FileSubcategorySelector.RefreshData();

            FileSubcategoryText.Text = null;
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

        private void PermissionList_PermissionsChanged(object sender, EventArgs e)
        {
            BindAllowLearnerSettings(ObjectIdentifier, ObjectType, PermissionList.GetFileClaims(), AllowLearnerToViewYes.Checked);
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

            SwitchToManualButton.Enabled = !string.IsNullOrEmpty(FileCategory.Value);

            BindAllowLearnerSettings(objectIdentifier, objectType, null, null);

            return (isValid, title);
        }

        public (bool IsValid, string Title) BindModelToControls(FileStorageModel model, Guid? caseId)
        {
            var (isValid, title) = Validate(model.ObjectType, model.ObjectIdentifier);
            if (!isValid)
                return (isValid, title);

            ObjectIdentifier = model.ObjectIdentifier;
            ObjectType = model.ObjectType;
            OriginalDocumentName = model.Properties.DocumentName;
            CaseId = caseId;

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

            FileSubcategorySelectorView.IsActive = true;

            FileSubcategorySelector.DocumentType = properties.Category;
            FileSubcategorySelector.RefreshData();
            FileSubcategorySelector.Value = properties.Subcategory;

            FileSubcategoryText.Text = null;

            SwitchToManualButton.Enabled = !string.IsNullOrEmpty(FileCategory.Value);

            FileDescription.Text = properties.Description;
            DocumentName.Text = properties.DocumentName;
            FileExpiry.Value = properties.Expiry?.UtcDateTime;
            FileReceived.Value = properties.Received?.UtcDateTime;
            FileAlternated.Value = properties.Alternated?.UtcDateTime;
            IsReviewed.Checked = properties.ReviewedTime.HasValue;
            IsApproved.Checked = properties.ApprovedTime.HasValue;

            BindAllowLearnerSettings(model.ObjectIdentifier, model.ObjectType, model.Claims, properties.AllowLearnerToView);

            return (isValid, title);
        }

        private void BindAllowLearnerSettings(Guid objectId, FileObjectType objectType, IEnumerable<FileClaim> fileClaims, bool? allowLearnerToView)
        {
            var caseEntity = CaseId != null
                ? ServiceLocator.IssueSearch.GetIssue(CaseId.Value)
                : objectType == FileObjectType.Issue
                        ? ServiceLocator.IssueSearch.GetIssue(objectId)
                        : null;

            var visible = caseEntity != null;

            AllowLearnerToViewPanel.Visible = visible;

            if (!visible)
            {
                AllowLearnerToViewYes.Checked = true;
                return;
            }

            if (IsPermissionDenied(caseEntity.TopicUserIdentifier, fileClaims))
            {
                AllowLearnerToViewPanel.Visible = false;
                AllowLearnerToViewYes.Checked = false;
                AllowLearnerToViewNo.Checked = true;
                return;
            }

            AllowLearnerToViewYes.Checked = allowLearnerToView == true;
            AllowLearnerToViewNo.Checked = allowLearnerToView != true;
        }

        private static bool IsPermissionDenied(Guid? topicUserId, IEnumerable<FileClaim> fileClaims)
        {
            if (topicUserId == null || fileClaims == null)
                return false;

            var groups = ServiceLocator.GroupSearch.GetGroups(new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = topicUserId
            });

            var userRoleIds = groups.Select(x => x.GroupIdentifier).ToArray();

            return ServiceLocator.StorageService.GetGrantStatus(topicUserId.Value, userRoleIds, fileClaims) != FileGrantStatus.Granted;
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
                Subcategory = GetOrCreateSubcategory(),
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

        private string GetOrCreateSubcategory()
        {
            if (FileSubcategorySelectorView.IsActive)
                return FileSubcategorySelector.Value;

            if (string.IsNullOrEmpty(FileCategory.Value))
                return null;

            var name = FileSubcategoryText.Text;
            var itemName = $"{FileCategory.Value}: {name}";

            var existItems = TCollectionItemSearch.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Assets_Files_Document_SubType,
                ItemName = itemName
            });

            if (existItems.Count > 0)
                return name;

            var collectionId = TCollectionSearch.BindFirst(
                x => (Guid?)x.CollectionIdentifier,
                new TCollectionFilter { CollectionName = CollectionName.Assets_Files_Document_SubType }
            );
            if (collectionId == null)
                return null;

            var nextSequence = TCollectionItemSearch.GetNextSequence(collectionId.Value, Organization.Identifier);
            var entity = new TCollectionItem
            {
                ItemName = itemName,
                ItemSequence = nextSequence,
                OrganizationIdentifier = Organization.Identifier,
                ItemIdentifier = UniqueIdentifier.Create(),
                CollectionIdentifier = collectionId.Value
            };

            TCollectionItemStore.Insert(entity);
            TCollectionItemCache.Refresh();

            return name;
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
                case FileObjectType.Standard:
                    return ValidateStandard(objectIdentifier);
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
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue))
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

        private (bool, string) ValidateStandard(Guid objectIdentifier)
        {
            var standard = ServiceLocator.StandardSearch.GetStandard(objectIdentifier);
            if (standard == null || standard.OrganizationIdentifier != Organization.Identifier)
                return (false, null);

            return (true, standard.ContentTitle);
        }
    }
}
