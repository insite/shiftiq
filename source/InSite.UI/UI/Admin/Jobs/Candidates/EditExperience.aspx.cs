using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Jobs.Candidates
{
    public partial class EditExperience : AdminBasePage
    {
        private Guid ExperienceId => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;

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

            var entity = TCandidateExperienceSearch.Select(ExperienceId);
            var userId = entity.UserIdentifier;

            if (!userId.HasValue || !ServiceLocator.PersonSearch.IsPersonExist(userId.Value, Organization.Identifier))
                HttpResponseHelper.Redirect("/");

            Detail.SetInputValues(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = TCandidateExperienceSearch.Select(ExperienceId);

            Detail.GetInputValues(entity);

            entity.WhenModified = DateTimeOffset.UtcNow;

            TCandidateExperienceStore.Update(entity);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                "modalManager.closeModal('refresh');",
                true
            );
        }
    }
}