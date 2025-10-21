using System;

using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Issues.Comments.Controls
{
    public partial class CommentTextEditor : BaseUserControl
    {
        #region Classes

        [Serializable]
        protected class ControlData
        {
            #region Properties

            public Guid IssueIdentifier { get; private set; }

            #endregion

            #region Construction

            public ControlData(VIssue issue)
            {
                IssueIdentifier = issue.IssueIdentifier;
            }

            #endregion
        }

        #endregion

        protected ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            private set => ViewState[nameof(CurrentData)] = value;
        }

        public string Text
        {
            get => CommentText.Text;
            set
            {
                if (CurrentData == null)
                    throw new ApplicationError("The contol is not initialized.");

                CommentText.Text = value;
            }
        }

        protected string TextEditorObject => $"commentTextEditor_{ClientID}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            TextDropZone.Attributes["class"] = $"form-group {TextDropZone.ClientID}";
        }

        public void LoadData(VIssue issue, VComment comment = null)
        {
            CurrentData = new ControlData(issue);

            CommentText.Text = comment?.CommentText;
        }
    }
}