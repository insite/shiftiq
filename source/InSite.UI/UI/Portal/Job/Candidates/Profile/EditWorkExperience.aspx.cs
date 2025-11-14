using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio
{
    public partial class EditWorkExperience : PortalBasePage
    {
        private Guid ExperienceId => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindModelToControls();
                PageHelper.AutoBindHeader(Page);
            }
        }

        private void BindModelToControls()
        {
            var entity = TCandidateExperienceSearch.Select(ExperienceId);

            if (entity == null || entity.UserIdentifier != User.UserIdentifier)
            {
                RedirectBack();
                return;
            }

            Detail.BindModelToControls(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = TCandidateExperienceSearch.Select(ExperienceId);
            entity.WhenModified = DateTimeOffset.UtcNow;

            Detail.BindControlsToModel(entity);

            TCandidateExperienceStore.Update(entity);

            RedirectBack();
        }

        private void RedirectBack()
        {
            HttpResponseHelper.Redirect("/ui/portal/job/candidates/profile/edit#work-experience");
        }
    }
}