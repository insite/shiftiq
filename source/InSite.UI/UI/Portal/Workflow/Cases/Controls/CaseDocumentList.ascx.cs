using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Portal.Issues.Controls
{
    public partial class CaseDocumentList : BaseUserControl
    {
        public event EventHandler FileUploaded;

        class FileItem
        {
            public string DownloadUrl { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string DocumentType { get; set; }
            public string DocumentSubtype { get; set; }
            public DateTimeOffset Uploaded { get; set; }
            public string Status { get; set; }
            public DateTimeOffset? Reviewed { get; set; }
            public DateTimeOffset? Approved { get; set; }
            public bool AllowLearnerToView { get; set; }
        }

        private Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AttachmentInput.FileUploaded += (x, y) => SaveFile();
        }

        public void BindFiles(Guid issueIdentifier, Guid? topicUserIdentifier)
        {
            IssueIdentifier = issueIdentifier;

            var items = new List<FileItem>();

            ReadIssueFiles(issueIdentifier, items);

            if (topicUserIdentifier.HasValue)
                ReadResponseFiles(topicUserIdentifier.Value, items);

            items.Sort((a, b) => b.Uploaded.CompareTo(a.Uploaded));

            BindItems(items);
        }

        private void BindItems(List<FileItem> items)
        {
            ListRepeater.DataSource = items;
            ListRepeater.DataBind();
            ListRepeater.Visible = items.Count > 0;
        }

        private void SaveFile()
        {
            var existing = ServiceLocator.IssueSearch.GetAttachment(IssueIdentifier, AttachmentInput.FileName);
            if (existing == null)
                AddNewFile();
            else
                UpdateExistingFile(existing.FileIdentifier);

            AttachmentInput.InputText = null;

            FileUploaded?.Invoke(this, new EventArgs());
        }

        private void AddNewFile()
        {
            var model = AttachmentInput.SaveFile(IssueIdentifier, FileObjectType.Issue);
            
            var allowLearnerToView = CaseAttachmentHelper.AllowLearnerToViewByIssue(IssueIdentifier);
            if (model.Properties.AllowLearnerToView != allowLearnerToView)
            {
                model.Properties.AllowLearnerToView = allowLearnerToView;
                ServiceLocator.StorageService.ChangeProperties(model.FileIdentifier, User.Identifier, model.Properties, false);
            }

            ApplyPermissionRule(model, Organization.Toolkits.Issues?.PortalUploadClaimGroups);

            var command = new AddAttachment(
                IssueIdentifier,
                AttachmentInput.FileName,
                Path.GetExtension(model.FileName),
                model.FileIdentifier,
                DateTimeOffset.UtcNow,
                User.UserIdentifier
            );

            ServiceLocator.SendCommand(command);
        }

        private void UpdateExistingFile(Guid existingFileIdentifier)
        {
            if (existingFileIdentifier != Guid.Empty)
                ServiceLocator.StorageService.Delete(existingFileIdentifier);

            var model = AttachmentInput.SaveFile(IssueIdentifier, FileObjectType.Issue);

            ApplyPermissionRule(model, Organization.Toolkits.Issues?.PortalUploadClaimGroups);

            var command = new ChangeAttachmentFile(IssueIdentifier, AttachmentInput.FileName, model.FileIdentifier);

            ServiceLocator.SendCommand(command);
        }

        private static void ReadIssueFiles(Guid issueIdentifier, List<FileItem> items)
        {
            var filter = new QIssueAttachmentFilter
            {
                IssueIdentifier = issueIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            var attachments = ServiceLocator.IssueSearch.GetAttachments(filter);

            attachments.Sort((a, b) => b.FileUploaded.CompareTo(a.FileUploaded));

            foreach (var attachment in attachments)
            {
                var item = GetItemFromIssue(attachment);
                if (item == null)
                    continue;

                items.Add(item);
            }
        }

        private static FileItem GetItemFromIssue(VIssueAttachment attachment)
        {
            var (status, model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, attachment.FileIdentifier);
            if (status != FileGrantStatus.Granted)
                return null;

            return FromModel(model);
        }

        private static void ReadResponseFiles(Guid userId, List<FileItem> items)
        {
            var responses = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.OrganizationIdentifier, userId);

            foreach (var response in responses)
                AddItemsFromResponse(items, response);
        }

        private static void AddItemsFromResponse(List<FileItem> items, ResponseSurveyUpload response)
        {
            var list = ServiceLocator.StorageService.ParseSurveyResponseAnswer(response.ResponseAnswerText);

            foreach (var responseFile in list)
            {
                var (status, model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, responseFile.FileIdentifier);
                if (status != FileGrantStatus.Granted)
                    continue;

                items.Add(FromModel(model));
            }
        }

        private static FileItem FromModel(FileStorageModel model)
        {
            return new FileItem
            {
                DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, true),
                FileSize = model.FileSize.Bytes().Humanize("#"),
                DocumentType = model.Properties.Category,
                DocumentSubtype = model.Properties.Subcategory,
                FileName = HttpUtility.HtmlEncode(model.Properties.DocumentName),
                Uploaded = model.Uploaded,
                Status = model.Properties.Status,
                Reviewed = model.Properties.ReviewedTime,
                Approved = model.Properties.ApprovedTime,
                AllowLearnerToView = model.Properties.AllowLearnerToView
            };
        }

        internal static void ApplyPermissionRule(FileStorageModel model, Guid[] claimGroups)
        {
            if (claimGroups == null || claimGroups.Length == 0)
                return;

            var claims = model.Claims?.ToList() ?? new List<FileClaim>();

            if (User != null)
            {
                claims.Add(new FileClaim
                {
                    ObjectIdentifier = User.Identifier,
                    ObjectType = FileClaimObjectType.Person
                });
            }

            foreach (var group in claimGroups)
            {
                claims.Add(new FileClaim
                {
                    ObjectIdentifier = group,
                    ObjectType = FileClaimObjectType.Group
                });
            }

            ServiceLocator.StorageService.ChangeClaims(model.FileIdentifier, claims);
        }
    }
}