using System;

using InSite.Application.Contents.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Contacts.Comments.Controls
{
    public partial class CommentTextEditor : BaseUserControl
    {
        public string Text
        {
            get => CommentText.Value;
            set => CommentText.Value = value;
        }

        public void LoadData(Guid user, QComment comment = null)
        {
            CommentUpload.FolderPath = $"/Users/{user}/CommentAttachments";
            CommentText.Value = comment?.CommentText;
        }
    }
}