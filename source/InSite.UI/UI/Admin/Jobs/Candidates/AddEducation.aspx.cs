using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Jobs.Candidates
{
    public partial class AddEducation : AdminBasePage
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

            Detail.BindDefaults();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = new TCandidateEducation
            {
                EducationIdentifier = UniqueIdentifier.Create(),
                UserIdentifier = CandidateId,
                WhenModified = DateTimeOffset.UtcNow
            };

            Detail.GetInputValues(entity);

            TCandidateEducationStore.Insert(entity);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                "modalManager.closeModal('refresh');",
                true
            );
        }
    }
}