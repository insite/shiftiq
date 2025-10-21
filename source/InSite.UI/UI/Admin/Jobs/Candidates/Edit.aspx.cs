using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Jobs.Candidates.Controls;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Jobs.Candidates
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/jobs/candidates/search";

        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["contact"], out var result) ? result : Guid.Empty;

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

            CancelButton.NavigateUrl = SearchUrl;

            LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            StatusAlert.AddMessage(AlertType.Success, "Your changes have been successfully saved");
        }

        private void LoadData()
        {
            var person = PersonSearch.Select(Organization.Identifier, UserIdentifier, x => x.User, x => x.HomeAddress);
            if (person == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var user = person.User;

            PageHelper.AutoBindHeader(this, qualifier: $"{user.FirstName} {user.LastName}");

            CandidateCard.SetInputValues(person, person.HomeAddress);
            LanguageAbility.LoadData(person);
            ContactCommentGrid.LoadData(user);

            ContactUploadsGrid.LoadData(UserIdentifier);
            ContactUploadsCard.Visible = ContactUploadsGrid.RowCount > 0;

            CandidateExperienceGrid.LoadData(UserIdentifier);
            CandidateEducationGrid.LoadData(UserIdentifier);

            AchievementGrid.LoadData(person.OrganizationIdentifier, person.UserIdentifier);
        }

        private void Save()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.User, x => x.HomeAddress);
            if (person == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var homeAddress = person.GetAddress(ContactAddressType.Home);

            CandidateCard.GetInputValues(person, homeAddress);

            PersonStore.Update(person);

            LanguageAbility.SaveData(person);
        }
    }
}