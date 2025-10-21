using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class AttachmentsTabsNav : BaseUserControl
    {
        #region Classes

        private class AttachmentUserModel
        {
            public Guid Thumbprint { get; set; }
            public string FullName { get; set; }
        }

        private class AttachmentUploadModel
        {
            public Guid UploadIdentifier { get; set; }
            public string Name { get; set; }
            public string NavigateUrl { get; set; }
            public int? ContentSize { get; set; }
        }

        private class AttachmentFileModel
        {
            #region Properties

            public Guid Identifier => _attachment.Identifier;
            public Guid Upload => _attachment.Upload;
            public int AssetNumber => _attachment.Asset;
            public int AssetVersion => _attachment.AssetVersion;
            public string Title => (_attachment.Content?.Title.Default).IfNullOrEmpty("(Untitled)");
            public string Condition => _attachment.Condition ?? "Unassigned";
            public string PublicationStatus => _attachment.PublicationStatus.GetDescription();
            public int QuestionsCount => _attachment.EnumerateAllVersions().Sum(x => x.QuestionIdentifiers.Count);
            public string PostedOn => _attachment.Uploaded.Format(CurrentSessionState.Identity.User.TimeZone);
            public string KeywordsJson => JsonConvert.SerializeObject(new[] { Title.ToUpperInvariant(), FileName.ToUpperInvariant() });

            public bool UploadExists { get; }
            public string FileName { get; }
            public string FileUrl { get; }
            public string FileSize { get; }
            public string Author { get; }
            public int ChangesCount { get; }

            #endregion

            #region Fields

            private readonly Attachment _attachment;

            #endregion

            #region Construction

            public AttachmentFileModel(Attachment attachment, AttachmentUploadModel upload, AttachmentUserModel author, int changesCount)
            {
                _attachment = attachment;

                UploadExists = upload != null;

                if (UploadExists)
                {
                    FileName = upload.Name;
                    FileUrl = "/files" + upload.NavigateUrl;
                    FileSize = (upload.ContentSize ?? 0).Bytes().Humanize("0.##");
                }
                else
                {
                    FileUrl = null;
                    FileName = FileSize = "N/A";
                }

                Author = author != null ? author.FullName : "N/A";

                ChangesCount = changesCount;
            }

            #endregion
        }

        private class AttachmentImageModel : AttachmentFileModel
        {
            #region Properties

            public string ImageResolution { get; }
            public string ImageDimensions { get; }
            public string Color { get; }

            #endregion

            #region Construction

            public AttachmentImageModel(Attachment attachment, AttachmentUploadModel upload, AttachmentUserModel author, int changesCount)
                : base(attachment, upload, author, changesCount)
            {
                var hasImageResolution = attachment.Image.Resolution > 0;
                var dimensions = $"{attachment.Image.Actual.Width:n0} x {attachment.Image.Actual.Height:n0} pixels";

                if (attachment.Image.TargetOnline?.HasValue == true)
                    dimensions += $"<br/>{attachment.Image.TargetOnline.Width:n0} x {attachment.Image.TargetOnline.Height:n0} pixels (online)";

                if (attachment.Image.TargetPaper?.HasValue == true)
                    dimensions += $"<br/>{attachment.Image.TargetPaper.Width:n0} x {attachment.Image.TargetPaper.Height:n0} pixels (paper)";

                if (attachment.Image.Resolution > 0)
                    dimensions += $"<br/>{Math.Round((decimal)attachment.Image.Actual.Width / attachment.Image.Resolution):n0} x {Math.Round((decimal)attachment.Image.Actual.Height / attachment.Image.Resolution):n0} inches";

                ImageResolution = hasImageResolution
                    ? attachment.Image.Resolution + " DPI"
                    : null;
                ImageDimensions = dimensions.ToString();
                Color = attachment.Image.IsColor ? "Color" : "Black and White";
            }

            #endregion
        }

        #endregion

        #region Properties

        public string KeywordInput
        {
            get => (string)ViewState[nameof(KeywordInput)];
            set => ViewState[nameof(KeywordInput)] = value;
        }

        private Guid? BankID
        {
            get => (Guid?)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(AttachmentsTabsNav).FullName;
            CommonScript.ContentKey = typeof(AttachmentsTabsNav).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (KeywordInput.IsNotEmpty())
            {
                var keywordInputId = NamingContainer.FindControl(KeywordInput)?.ClientID ?? KeywordInput;

                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "reg_keyword",
                    $"attachmentsTabsNav.registerKeywordInput({HttpUtility.JavaScriptStringEncode(Nav.ClientID, true)},{HttpUtility.JavaScriptStringEncode(keywordInputId, true)});"
                    + $"attachmentsTabsNav.registerImages({HttpUtility.JavaScriptStringEncode(Nav.ClientID, true)});",
                    true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Data binding

        public void LoadData(Guid bankId, IEnumerable<Attachment> attachments, ReturnUrl returnUrl, bool canWrite)
        {
            BankID = bankId;
            CanWrite = canWrite;
            _returnUrl = returnUrl;

            var allUploads = UploadSearch
                .Bind(x => new AttachmentUploadModel
                {
                    UploadIdentifier = x.UploadIdentifier,
                    Name = x.Name,
                    NavigateUrl = x.NavigateUrl,
                    ContentSize = x.ContentSize,
                }, attachments)
                .ToDictionary(x => x.UploadIdentifier);

            var allUsers = UserSearch
                .Bind(
                    x => new AttachmentUserModel
                    {
                        Thumbprint = x.UserIdentifier,
                        FullName = x.FullName
                    },
                    new UserFilter
                    {
                        IncludeUserIdentifiers = attachments.Select(x => x.Author).Distinct().ToArray()
                    })
                .ToDictionary(x => x.Thumbprint);

            var events = AttachmentHelper.GetChanges(bankId)
                .SelectMany(e => AttachmentHelper.GetAttachmentIdentifier(e))
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var images = new List<AttachmentFileModel>();
            var docs = new List<AttachmentFileModel>();
            var others = new List<AttachmentFileModel>();

            foreach (var attachment in attachments)
            {
                var isImage = attachment.Type == AttachmentType.Image;
                var isDoc = attachment.Type == AttachmentType.Document;

                var upload = allUploads.GetOrDefault(attachment.Upload);
                var user = allUsers.GetOrDefault(attachment.Author);
                var changes = attachment.EnumerateAllVersions().Select(v => events.GetOrDefault(v.Identifier, 0)).Sum();
                var model = isImage
                    ? new AttachmentImageModel(attachment, upload, user, changes)
                    : new AttachmentFileModel(attachment, upload, user, changes);

                //if (isKeywordDefined && !model.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) && !model.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                //    continue;

                if (isImage)
                    images.Add(model);
                else if (isDoc)
                    docs.Add(model);
                else
                    others.Add(model);
            }

            ImageTab.SetTitle("Images", images.Count);
            ImageTab.Visible = images.Count > 0;
            ImageRepeater.DataSource = images.OrderBy(x => x.UploadExists).ThenBy(x => x.Title);
            ImageRepeater.DataBind();

            DocumentTab.SetTitle("Documents", docs.Count);
            DocumentTab.Visible = docs.Count > 0;
            DocumentRepeater.DataSource = docs.OrderBy(x => x.UploadExists).ThenBy(x => x.Title);
            DocumentRepeater.DataBind();

            OtherTab.SetTitle("Others", others.Count);
            OtherTab.Visible = others.Count > 0;
            OtherRepeater.DataSource = others.OrderBy(x => x.UploadExists).ThenBy(x => x.Title);
            OtherRepeater.DataBind();
        }

        public bool SelectTab(TabType tab)
        {
            NavItem navItem = null;

            if (tab == TabType.Image)
                navItem = ImageTab;
            else if (tab == TabType.Document)
                navItem = DocumentTab;
            else if (tab == TabType.Other)
                navItem = OtherTab;

            if (navItem != null && navItem.Visible)
            {
                navItem.IsSelected = true;
                return true;
            }

            return false;
        }

        #endregion

        #region Helper methods

        protected string GetEditUrl() =>
            _returnUrl.GetRedirectUrl($"/ui/admin/assessments/attachments/change?bank={BankID}&attachment={Eval("Identifier")}");

        protected string GetRemoveUrl() =>
            _returnUrl.GetRedirectUrl($"/admin/assessments/attachments/delete?bank={BankID}&attachment={Eval("Identifier")}");

        protected string GetHistoryUrl() =>
            _returnUrl.GetRedirectUrl($"/ui/admin/assessments/attachments/history?bank={BankID}&attachment={Eval("Identifier")}&version=all");

        protected string GetUsageUrl() =>
            _returnUrl.GetRedirectUrl($"/ui/admin/assessments/attachments/usage?bank={BankID}&attachment={Eval("Identifier")}&version=all");

        #endregion
    }
}