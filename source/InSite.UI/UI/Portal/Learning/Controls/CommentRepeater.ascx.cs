using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Contents.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class CommentRepeater : BaseUserControl
    {
        #region Properties

        public int Count => Repeater.Items.Count;

        protected Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        #endregion

        #region Data binding

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Repeater.ItemCreated += Repeater_ItemCreated;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var delete = (IconButton)e.Item.FindControl("DeleteComment");
            delete.Click += Delete_Click;
            delete.Visible = Identity.IsAdministrator;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var comment = (VComment)e.Item.DataItem;
            var delete = (IconButton)e.Item.FindControl("DeleteComment");
            delete.CommandArgument = comment.CommentIdentifier.ToString();
            delete.ConfirmText = "Are you sure you want to delete this comment?";
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var id = Guid.Parse(((IconButton)sender).CommandArgument);
            var comment = QCommentSearch.Bind(x => x, x => x.CommentIdentifier == id).FirstOrDefault();
            if (comment == null)
                return;

            QCommentStore.Delete(comment);
            SetInputValues();
        }

        public void LoadData(Guid course)
        {
            CourseIdentifier = course;

            SetInputValues();
        }

        #endregion

        #region Getting and setting input values

        private void SetInputValues()
        {
            Repeater.DataSource = null;

            var comments = GetComments();

            Repeater.DataSource = comments.OrderByDescending(x => x.CommentPosted);
            Repeater.DataBind();
        }

        private List<VComment> GetComments()
        {
            return QCommentSearch.Search(Organization.Identifier, CourseIdentifier);
        }

        #endregion

        #region Helper methods

        protected string GetTimestamp(object item)
        {
            var comment = (VComment)item;
            var time = LocalizeTime(comment.CommentPosted, null, false);
            return $"<span title='{time}'>posted {time}</span>";
        }

        protected string GetCommentHtml(object item)
        {
            var comment = (VComment)item;
            if (comment.CommentText != null)
                return Markdown.ToHtml(comment.CommentText);
            return string.Empty;
        }

        #endregion
    }
}