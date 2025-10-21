using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class DocumentList : BaseUserControl
    {
        #region Classes

        class FileItem
        {
            public Guid FileIdentifier { get; set; }
            public bool HasFile { get; set; }
            public string DownloadUrl { get; set; }
            public string DeleteUrl { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string FileStatus { get; set; }
            public string DocumentDescription { get; set; }
            public string DocumentName { get; set; }
            public string DocumentType { get; set; }
            public bool IsPublicAccess { get; set; }
            public string IssueUrl { get; set; }
            public string IssueName { get; set; }
            public string ResponseSource { get; set; }
            public string ResponseQuestionNumber { get; set; }
            public string UploadedTime { get; set; }
            public string UploadedBy { get; set; }
            public string ReviewedTime { get; set; }
            public string ReviewedBy { get; set; }
            public string ApprovedTime { get; set; }
            public string ApprovedBy { get; set; }
            public DateTimeOffset FileUploaded { get; set; }
        }

        #endregion

        #region Properties and fields

        protected bool IsIssue { get; set; }
        protected bool IsResponse { get; set; }

        private Dictionary<Guid, string> _users;
        private Dictionary<Guid, string> _surveys;

        #endregion

        #region Public methods

        public bool BindFiles(List<FileStorageModel> files)
        {
            var items = files.Select(GetFileItem).ToList();

            return BindItems(items);
        }

        public bool BindResponseFiles(Guid userId)
        {
            IsResponse = true;

            var responses = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.OrganizationIdentifier, userId);
            var items = new List<FileItem>();

            foreach (var response in responses)
                AddItemsFromResponse(items, response);

            return BindItems(items);
        }

        public bool BindIssueFiles(Guid userId)
        {
            IsIssue = true;

            var filter = new QIssueAttachmentFilter
            {
                TopicUserIdentifier = userId,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            var attachments = ServiceLocator.IssueSearch.GetAttachments(filter);
            var items = new List<FileItem>();

            foreach (var attachment in attachments)
            {
                var item = GetItemFromIssue(attachment);
                if (item == null)
                    continue;

                items.Add(item);
            }

            return BindItems(items);
        }

        private FileItem GetItemFromIssue(VIssueAttachment attachment)
        {
            var (status, file) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, attachment.FileIdentifier);

            if (status == FileGrantStatus.Denied)
                return null;

            var item = status != FileGrantStatus.Granted
                ? new FileItem()
                : GetFileItem(file);

            item.FileName = attachment.FileName;
            item.DocumentName = attachment.FileName;
            item.IssueUrl = $"/ui/admin/workflow/cases/outline?case={attachment.IssueIdentifier}";
            item.IssueName = $"#{attachment.IssueNumber}: {attachment.IssueType} - {attachment.IssueTitle}";
            item.UploadedTime = attachment.FileUploaded.FormatDateOnly(User.TimeZone);
            item.FileUploaded = attachment.FileUploaded;
            item.UploadedBy = attachment.InputterUserName;

            return item;
        }

        #endregion

        #region Response files

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
            var (status, file) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, fileIdentifier);

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

            var item = GetFileItem(file);
            item.DeleteUrl = null;
            item.UploadedTime = (response.ResponseSessionStarted ?? file.Uploaded).FormatDateOnly(User.TimeZone);
            item.FileUploaded = response.ResponseSessionStarted ?? file.Uploaded;
            item.UploadedBy = GetUserName(response.RespondentUserIdentifier);
            item.ResponseSource = response.SurveyFormName;
            item.ResponseQuestionNumber = GetSurveyQuestionNumber(response.SurveyFormIdentifier, response.SurveyQuestionIdentifier);

            return item;
        }

        private string GetSurveyQuestionNumber(Guid surveyIdentifier, Guid questionIdentifier)
        {
            if (_surveys == null)
                _surveys = new Dictionary<Guid, string>();

            if (_surveys.TryGetValue(questionIdentifier, out var number))
                return number;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyIdentifier);
            var questions = survey.Form.Questions.Where(x => x.HasInput).ToList();
            var groupCode = string.Empty;

            foreach (var question in questions)
            {
                string code;
                if (question.IsNested)
                    code = groupCode;
                else
                {
                    code = question.Code.HasValue()
                        ? question.Code
                        : question.Sequence.ToString();

                    groupCode = code;
                }

                _surveys.Add(question.Identifier, code);
            }

            return _surveys.TryGetValue(questionIdentifier, out number) ? number : string.Empty;
        }

        #endregion

        #region Common helpers

        private bool BindItems(List<FileItem> items)
        {
            items = items
            .OrderBy(item => item.DocumentType == null)
            .ThenBy(item => item.DocumentType)
            .ThenBy(item => item.FileStatus)
            .ThenByDescending(item => item.FileUploaded)
            .ToList();

            ListRepeater.DataSource = items;
            ListRepeater.DataBind();
            ListRepeater.Visible = items.Count > 0;

            return items.Count > 0;
        }

        private FileItem GetFileItem(FileStorageModel model)
        {
            var item = new FileItem
            {
                FileIdentifier = model.FileIdentifier,
                HasFile = true,
                DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false),
                DeleteUrl = $"/ui/admin/contacts/people/delete-attachment?contact={model.ObjectIdentifier}&file={model.FileIdentifier}",
                FileName = model.FileName,
                FileSize = model.FileSize.Bytes().Humanize("#"),
                FileStatus = model.Properties.Status,
                DocumentName = model.Properties.DocumentName,
                DocumentType = model.Properties.Category,
                DocumentDescription = StringHelper.Snip(model.Properties.Description,100),
                IsPublicAccess = model.Claims == null || !model.Claims.Any(),
                UploadedTime = model.Uploaded.FormatDateOnly(User.TimeZone),
                FileUploaded = model.Uploaded,
                UploadedBy = GetUserName(model.UserIdentifier),
                ReviewedTime = model.Properties.ReviewedTime?.FormatDateOnly(User.TimeZone),
                ReviewedBy = GetUserName(model.Properties.ReviewedUserIdentifier),
                ApprovedTime = model.Properties.ApprovedTime?.FormatDateOnly(User.TimeZone),
                ApprovedBy = GetUserName(model.Properties.ApprovedUserIdentifier)
            };

            return item;
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

        #endregion
    }
}