using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contents.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Contacts.Comments.Controls
{
    public partial class CommentRepeater : BaseUserControl
    {
        #region Properties

        public int Count { get; private set; }

        protected Guid UserIdentifier
        {
            get { return (Guid)ViewState[nameof(UserIdentifier)]; }
            set { ViewState[nameof(UserIdentifier)] = value; }
        }

        private Guid OrganizationIdentifier
        {
            get { return (Guid)ViewState[nameof(OrganizationIdentifier)]; }
            set { ViewState[nameof(OrganizationIdentifier)] = value; }
        }

        private string ExportFileName => "Comments";

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += Repeater_DataBinding;

            DownloadXlsxButton.Click += DownloadXlsxButton_Click;
        }

        #endregion

        #region Load data

        public bool LoadData(Guid user, Guid organization)
        {
            UserIdentifier = user;
            OrganizationIdentifier = organization;

            Repeater.DataBind();

            AddCommentButton.NavigateUrl = new ReturnUrl(Request.RawUrl)
                .GetRedirectUrl($"/ui/admin/contacts/comments/author?contact={user}");

            return Repeater.Items.Count > 0;
        }

        #endregion

        #region Event handling

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            SetInputValues();
        }

        private void DownloadXlsxButton_Click(object sender, EventArgs e)
        {
            ExportToXlsx();
        }

        #endregion

        #region Binding

        private void SetInputValues()
        {
            Repeater.DataSource = null;

            var comments = PersonCommentSummarySearch.SelectForCommentRepeater(UserIdentifier, OrganizationIdentifier);
            Repeater.DataSource = comments;
            Count = comments.Count;

            DownloadXlsxButton.Visible = Count > 0;
        }

        protected string GetRedirectUrl(string url, params object[] args)
        {
            return new ReturnUrl(Request.RawUrl)
                .GetRedirectUrl(string.Format(url, args));
        }

        #endregion

        #region Data binding evaluation

        protected string GetAuthorName(object item)
        {
            var comment = (VComment)item;
            if (comment.AuthorUserName != null)
                return comment.AuthorUserName;
            return "Someone";
        }

        protected string GetCommentHtml(object item)
        {
            var comment = (VComment)item;
            if (comment.CommentText != null)
                return Markdown.ToHtml(comment.CommentText);
            return string.Empty;
        }

        protected string GetTimestamp(object item)
        {
            var comment = (VComment)item;
            return $"<span title='{LocalizeTime(comment.CommentPosted, null, false)}'>commented " + TimeZones.Format(comment.CommentPosted, CurrentSessionState.Identity.User.TimeZone) + "</span>";
        }

        #endregion

        #region Export to XLSX

        private void ExportToXlsx()
        {
            var data = GetCommentsXlsx();
            if (data == null)
                return;

            var filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}",
                StringHelper.Sanitize(ExportFileName, '-'), DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", data);
        }

        private byte[] GetCommentsXlsx()
        {
            var list = PersonCommentSummarySearch.SelectForCommentRepeater(UserIdentifier, OrganizationIdentifier).Select(x => new
            {
                Posted = x.CommentPosted,
                AuthorName = x.AuthorUserName,
                AuthorEmail = x.AuthorEmail,
                Comments = x.CommentText
            }).ToList();

            if (list.Count == 0)
                return null;

            var helper = new XlsxExportHelper();

            helper.Map("Posted", "Posted", 30, HorizontalAlignment.Left);
            helper.Map("AuthorName", "Author Name", 30, HorizontalAlignment.Left);
            helper.Map("AuthorEmail", "Author Email", 30, HorizontalAlignment.Left);
            helper.Map("Comments", "Comments", 60, HorizontalAlignment.Left);

            return helper.GetXlsxBytes(list, ExportFileName);
        }

        #endregion
    }
}