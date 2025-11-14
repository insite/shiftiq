using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Jobs.Candidates
{
    public partial class AddComment : AdminBasePage
    {
        private Guid CandidateId => Guid.TryParse(Request["candidate"], out var value) ? value : Guid.Empty;

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

            if (!ServiceLocator.PersonSearch.IsPersonExist(CandidateId, Organization.Identifier))
                HttpResponseHelper.Redirect("/");

            Detail.SetDefaultInputValues(CandidateId);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var entity = new VCandidateComment
            {
                CommentIdentifier = UniqueIdentifier.Create(),
                CandidateUserIdentifier = CandidateId,
                AuthorUserIdentifier = User.Identifier,
                CommentModified = DateTimeOffset.UtcNow,
                OrganizationIdentifier = Organization.Identifier,
                AuthorName = User.FullName,
                ContainerType = "Person"
            };

            Detail.GetInputValues(entity);

            TCandidateCommentStore.Insert(entity);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                "modalManager.closeModal('refresh');",
                true
            );
        }
    }
}