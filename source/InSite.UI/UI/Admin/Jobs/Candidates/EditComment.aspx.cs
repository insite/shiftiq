using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Jobs.Candidates
{
    public partial class EditComment : AdminBasePage
    {
        private Guid CommentId => Guid.TryParse(Request["comment"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var comment = TCandidateCommentSearch.Select(CommentId);
            var userId = comment.CandidateUserIdentifier;

            if (!userId.HasValue || !ServiceLocator.PersonSearch.IsPersonExist(userId.Value, Organization.Identifier))
                HttpResponseHelper.Redirect("/");

            Detail.SetInputValues(comment);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = TCandidateCommentSearch.Select(CommentId);

            Detail.GetInputValues(entity);

            entity.CommentModified = DateTime.UtcNow;

            TCandidateCommentStore.Update(entity);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                "modalManager.closeModal('refresh');",
                true
            );
        }
    }
}