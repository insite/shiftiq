using System;

using InSite.Application.Contents.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class CommentTextEditor : BaseUserControl
    {
        public string Text
        {
            get => CommentText.Text;
            set => CommentText.Text = value;
        }

        protected string TextEditorObject => $"commentTextEditor_{ClientID}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            TextDropZone.Attributes["class"] = $"form-group {TextDropZone.ClientID}";
        }

        public void LoadData(VComment comment = null)
        {
            CommentText.Text = comment?.CommentText;
        }
    }
}