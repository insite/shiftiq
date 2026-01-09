using System;
using System.Collections.Generic;

using Humanizer;

using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Contacts.Referral.Controls
{
    public partial class PortalDocumentList : BaseUserControl
    {
        class FileItem
        {
            public string DownloadUrl { get; set; }
            public string DocumentName { get; set; }
            public string FileSize { get; set; }
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

            fileItems.Sort((a, b) => a.DocumentName.CompareTo(b.DocumentName));

            return fileItems;
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
                    FileSize = model.FileSize.Bytes().Humanize("#")
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
                        FileSize = model.FileSize.Bytes().Humanize("#")
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
                    FileSize = model.FileSize.Bytes().Humanize("#")
                };

                fileItems.Add(item);
            }

        }
    }
}