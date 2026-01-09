using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Issues.Outlines.Controls
{
    public partial class CaseDocumentList : BaseUserControl
    {
        class FileItem
        {
            public Guid FileIdentifier { get; set; }
            public bool HasFile { get; set; }
            public string DownloadUrl { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string FileStatus { get; set; }
            public string DocumentType { get; set; }
            public string DocumentDescription { get; set; }
            public bool IsPublicAccess { get; set; }
            public bool AllowLearnerToView { get; set; }
            public string UploadedTime { get; set; }
            public DateTimeOffset FileUploaded { get; set; }
            public string UploadedBy { get; set; }
            public string ReviewedTime { get; set; }
            public string ReviewedBy { get; set; }
            public string ApprovedTime { get; set; }
            public string ApprovedBy { get; set; }
            public string Source { get; set; }
            public string AccessList { get; set; }
        }

        protected Guid IssueIdentifier { get; private set; }

        private Dictionary<Guid, string> _users;

        public void BindIssueFiles(Guid issueIdentifier, Guid? respondentUserId)
        {
            IssueIdentifier = issueIdentifier;

            var items = new List<FileItem>();

            AddIssueAttachments(items);

            if (respondentUserId.HasValue)
                AddResponseFiles(respondentUserId.Value, items);

            items = items
            .OrderBy(item => item.DocumentType == null)
            .ThenBy(item => item.DocumentType)
            .ThenBy(item => item.FileStatus)
            .ThenByDescending(item => item.FileUploaded)
            .ToList();

            BindItems(items);
        }

        private void BindItems(List<FileItem> items)
        {
            ListRepeater.DataSource = items;
            ListRepeater.DataBind();
            ListRepeater.Visible = items.Count > 0;

            RepeaterNoItems.Visible = items.Count == 0;
        }

        private void AddIssueAttachments(List<FileItem> items)
        {
            var filter = new QIssueAttachmentFilter
            {
                IssueIdentifier = IssueIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            var attachments = ServiceLocator.IssueSearch.GetAttachments(filter);
            foreach (var attachment in attachments)
            {
                var item = GetItemFromIssue(attachment);
                if (item == null)
                    continue;

                items.Add(item);
            }
        }

        private FileItem GetItemFromIssue(VIssueAttachment attachment)
        {
            var (status, fileModel) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, attachment.FileIdentifier);

            if (status == FileGrantStatus.Denied)
                return null;

            var item = status != FileGrantStatus.Granted
                ? new FileItem()
                : GetFileItemFromModel(fileModel);

            item.FileName = HttpUtility.HtmlEncode(attachment.FileName);
            item.UploadedTime = FormatDate(attachment.FileUploaded);
            item.FileUploaded = attachment.FileUploaded;
            item.UploadedBy = attachment.InputterUserName;
            item.Source = "Case";
            item.AllowLearnerToView = fileModel.Properties.AllowLearnerToView;

            return item;
        }

        private void AddResponseFiles(Guid respondentUserId, List<FileItem> items)
        {
            var responses = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.OrganizationIdentifier, respondentUserId, true);
            foreach (var response in responses)
                AddItemsFromResponse(items, response);
        }

        private void AddItemsFromResponse(List<FileItem> items, ResponseSurveyUpload response)
        {
            var list = ServiceLocator.StorageService.ParseSurveyResponseAnswer(response.ResponseAnswerText);

            foreach (var responseFile in list)
            {
                var item = GetFileItemByUrl(response, responseFile.FileIdentifier, responseFile.FileName);
                if (item != null)
                    items.Add(item);
            }
        }

        private FileItem GetFileItemByUrl(ResponseSurveyUpload response, Guid fileIdentifier, string fileName)
        {
            var (status, model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, fileIdentifier);

            switch (status)
            {
                case FileGrantStatus.Denied:
                    return null;
                case FileGrantStatus.NoFile:
                    return new FileItem { FileName = fileName };
                case FileGrantStatus.Granted:
                    break;
                default:
                    throw new ArgumentException($"Unsupported file status: {status}");
            }

            var item = GetFileItemFromModel(model);
            item.FileName = HttpUtility.HtmlEncode(model.Properties.DocumentName);
            item.UploadedTime = (response.ResponseSessionStarted ?? model.Uploaded).FormatDateOnly(User.TimeZone);
            item.FileUploaded = response.ResponseSessionStarted ?? model.Uploaded;
            item.UploadedBy = GetUserName(response.RespondentUserIdentifier);
            item.Source = "Response";
            item.AllowLearnerToView = model.Properties.AllowLearnerToView;

            return item;
        }

        private FileItem GetFileItemFromModel(FileStorageModel model)
        {
            var item = new FileItem
            {
                FileIdentifier = model.FileIdentifier,
                DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false),
                FileSize = model.FileSize.Bytes().Humanize("#"),
                FileStatus = model.Properties.Status,
                DocumentType = model.Properties.Category,
                DocumentDescription = StringHelper.Snip(model.Properties.Description, 100),
                IsPublicAccess = model.Claims == null || !model.Claims.Any(),
                ReviewedTime = FormatDate(model.Properties.ReviewedTime),
                ReviewedBy = GetUserName(model.Properties.ReviewedUserIdentifier),
                ApprovedTime = FormatDate(model.Properties.ApprovedTime),
                ApprovedBy = GetUserName(model.Properties.ApprovedUserIdentifier)
            };

            var groupIdentifiers = model.Claims
                .Where(x => x.ObjectType == FileClaimObjectType.Group)
                .Select(x => x.ObjectIdentifier)
                .Distinct()
                .ToList();

            var groupNames = ServiceLocator.GroupSearch
                .BindGroups(
                    g => g.GroupName,
                    g => groupIdentifiers.Contains(g.GroupIdentifier))
                .OrderBy(g => g)
                .ToList();

            var userIdentifiers = model.Claims
                .Where(x => x.ObjectType == FileClaimObjectType.Person)
                .Select(x => x.ObjectIdentifier)
                .Distinct()
                .ToList();

            var userNames = UserSearch
                .BindUsers(
                    u => u.FullName,
                    u => userIdentifiers.Contains(u.UserIdentifier))
                .OrderBy(u => u)
                .ToList();

            if (groupNames.Any() || userNames.Any())
                item.AccessList = string.Join(", ", groupNames.Union(userNames));

            item.HasFile = System.IO.Path.HasExtension(item.DownloadUrl);

            return item;
        }

        private string FormatDate(DateTimeOffset? date)
        {
            return date?.FormatDateOnly(User.TimeZone);
        }

        private string GetUserName(Guid? userId)
        {
            if (userId == null)
                return null;

            if (_users == null)
                _users = new Dictionary<Guid, string>();

            if (_users.TryGetValue(userId.Value, out var name))
                return name;

            var user = UserSearch.Select(userId.Value);
            name = user?.FullName ?? "Someone";

            _users.Add(userId.Value, name);

            return name;
        }
    }
}