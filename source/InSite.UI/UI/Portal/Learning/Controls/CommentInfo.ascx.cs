using System;
using System.Web.UI;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class CommentInfo : UserControl
    {
        public Guid IssueIdentifier { get; set; }

        public Guid CommentIdentifier { get; set; }

        public Guid OrganizationIdentifier
            => CurrentSessionState.Identity.Organization.OrganizationIdentifier;

        public string CommentTextValue
            => CommentText.Text;

        public bool? CommentPreviousPrivacy
        {
            get => (bool?)ViewState[nameof(CommentPreviousPrivacy)];
            set => ViewState[nameof(CommentPreviousPrivacy)] = value;
        }

        public void Clear()
        {
            CommentText.Text = string.Empty;
        }
    }
}