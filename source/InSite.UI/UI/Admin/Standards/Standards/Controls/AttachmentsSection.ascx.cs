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


namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class AttachmentsSection : BaseUserControl
    {
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

        private Guid StandardIdentifier
        {
            get => (Guid)ViewState[nameof(StandardIdentifier)];
            set => ViewState[nameof(StandardIdentifier)] = value;
        }

        private Guid? RespondentUserId
        {
            get => (Guid?)ViewState[nameof(RespondentUserId)];
            set => ViewState[nameof(RespondentUserId)] = value;
        }

        private Dictionary<Guid, string> _users;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        private void IssueFileRequirementList_Changed(object sender, EventArgs e)
        {
            BindModelToControls(StandardIdentifier);
        }

        public void BindModelToControls(Guid standardIdentifier)
        {
            StandardIdentifier = standardIdentifier;

            var files = GetAttachments(StandardIdentifier);

            BindFiles(files);

            AddAttachmentButton.NavigateUrl = $"/ui/admin/assets/files/upload?standard={standardIdentifier}";
        }

        private List<FileStorageModel> GetAttachments(Guid standardIdentifier)
            => ServiceLocator.StorageService.GetGrantedFiles(Identity, standardIdentifier);

        public bool BindFiles(List<FileStorageModel> files)
        {
            var items = files.Select(GetFileItem).ToList();

            return BindItems(items);
        }

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
                DocumentDescription = StringHelper.Snip(model.Properties.Description, 100),
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

    }
}