using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio
{
    public partial class AddWorkExperience : PortalBasePage
    {
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

            BindModelToControls();
            PageHelper.AutoBindHeader(Page);
        }

        private void BindModelToControls()
        {
            Detail.BindDefaults();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new TCandidateExperience
            {
                ExperienceIdentifier = UniqueIdentifier.Create(),
                UserIdentifier = User.Identifier,
                WhenModified = DateTimeOffset.UtcNow
            };

            Detail.BindControlsToModel(entity);

            TCandidateExperienceStore.Insert(entity);

            RedirectBack();
        }

        private void RedirectBack()
        {
            HttpResponseHelper.Redirect("/ui/portal/job/candidates/profile/edit#work-experience");
        }
    }
}