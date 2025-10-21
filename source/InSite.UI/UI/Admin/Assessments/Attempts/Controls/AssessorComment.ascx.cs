using System;
using System.Web.WebPages;

using InSite.Application.Contents.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class AssessorComment : BaseUserControl
    {
        public string Title
        {
            get => ControlTitle.InnerText;
            set => ControlTitle.InnerText = value;
        }

        public QComment Comment { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Comment == null || Comment.CommentText.IsEmpty())
                return;

            CommentText.Text = Comment?.CommentText ?? string.Empty;
            Visible = true;
        }
    }
}