using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Contacts.Referral.Controls
{
    public partial class PortalDocumentList : BaseUserControl
    {
        class FileItem
        {
            public string DownloadUrl { get; set; }
            public string DocumentName { get; set; }
            public string FileSize { get; set; }
            public string DocumentType { get; set; }
            public DateTimeOffset Uploaded { get; set; }
            public Guid UploadedBy { get; set; }
            public bool IsAdministrator { get; set; }
            public bool IsApplicant { get; set; }
        }

        public void BindFiles(Guid userIdentifier)
        {
            var fileItems = GetFileItems(userIdentifier);

            NoDocuments.Visible = fileItems.Count == 0;

            DocumentRepeater.Visible = fileItems.Count > 0;
            DocumentRepeater.DataSource = fileItems;
            DocumentRepeater.DataBind();
        }

        private List<FileItem> GetFileItems(Guid userIdentifier)
        {
            var fileItems = new List<FileItem>();

            AddPersonFiles(userIdentifier, fileItems);
            AddResponseFiles(userIdentifier, fileItems);
            AddIssueFiles(userIdentifier, fileItems);

            CheckPersonType(fileItems);

            fileItems.Sort((a, b) => a.DocumentName.CompareTo(b.DocumentName));

            return fileItems;
        }

        private void CheckPersonType(List<FileItem> fileItems)
        {
            var userIds = fileItems.Select(x => x.UploadedBy).Distinct().ToArray();

            var filter = new QPersonFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifiers = userIds
            };

            var users = ServiceLocator.PersonSearch
                .GetPersons(filter)
                .Select(x => new
                {
                    UserId = x.UserIdentifier,
                    x.IsAdministrator,
                    x.IsLearner
                })
                .ToDictionary(x => x.UserId);

            foreach (var fileItem in fileItems)
            {
                users.TryGetValue(fileItem.UploadedBy, out var uploadedBy);

                fileItem.IsAdministrator = uploadedBy != null && uploadedBy.IsAdministrator;
                fileItem.IsApplicant = uploadedBy != null && uploadedBy.IsLearner;
            }
        }

        private void AddPersonFiles(Guid userIdentifier, List<FileItem> fileItems)
        {
            var personFiles = ServiceLocator.StorageService.GetGrantedFiles(Identity, userIdentifier);

            foreach (var model in personFiles)
            {
                var item = new FileItem
                {
                    DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, true),
                    DocumentName = model.Properties.DocumentName,
                    FileSize = model.FileSize.Bytes().Humanize("#"),
                    DocumentType = model.Properties.Category,
                    Uploaded = model.Uploaded,
                    UploadedBy = model.UserIdentifier
                };

                fileItems.Add(item);
            }
        }

        private void AddResponseFiles(Guid userIdentifier, List<FileItem> fileItems)
        {
            var responses = ServiceLocator.SurveySearch.GetResponseSurveyUploads(Organization.OrganizationIdentifier, userIdentifier);

            foreach (var response in responses)
            {
                var list = ServiceLocator.StorageService.ParseSurveyResponseAnswer(response.ResponseAnswerText);

                foreach (var responseFile in list)
                {
                    var (status, model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, responseFile.FileIdentifier);

                    if (status != FileGrantStatus.Granted)
                        continue;

                    var item = new FileItem
                    {
                        DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, true),
                        DocumentName = model.Properties.DocumentName,
                        FileSize = model.FileSize.Bytes().Humanize("#"),
                        DocumentType = model.Properties.Category,
                        Uploaded = model.Uploaded,
                        UploadedBy = model.UserIdentifier
                    };

                    fileItems.Add(item);
                }
            }
        }

        private void AddIssueFiles(Guid userIdentifier, List<FileItem> fileItems)
        {
            var filter = new QIssueAttachmentFilter
            {
                TopicUserIdentifier = userIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            var attachments = ServiceLocator.IssueSearch.GetAttachments(filter);

            foreach (var attachment in attachments)
            {
                var (status, model) = ServiceLocator.StorageService.GetFileAndAuthorize(Identity, attachment.FileIdentifier);

                if (status != FileGrantStatus.Granted)
                    continue;

                var item = new FileItem
                {
                    DownloadUrl = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, true),
                    DocumentName = attachment.FileName,
                    FileSize = model.FileSize.Bytes().Humanize("#"),
                    DocumentType = model.Properties.Category,
                    Uploaded = model.Uploaded,
                    UploadedBy = model.UserIdentifier
                };

                fileItems.Add(item);
            }
        }

        protected string FormatUploaded()
        {
            var item = (FileItem)Page.GetDataItem();
            return TimeZones.FormatDateOnly(item.Uploaded, User.TimeZone);
        }
    }
}