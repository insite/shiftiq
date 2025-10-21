using System;
using System.Web.UI;

using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio
{
    public partial class Edit : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            BindModelToControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var isLocked = Experience.ItemsCount == 0 || Education.ItemsCount == 0;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Edit),
                "set_locked",
                $"profileDetail.setLocked({(isLocked ? "true" : "false")});",
                true);
        }

        private void BindModelToControls()
        {
            var person = PersonSearch.Select(Organization.Identifier, User.Identifier, x => x.User, x => x.HomeAddress);

            ShowWarnings(person);

            JobInterest.BindModelToControls(person);
            LanguageAbility.BindModelToControls(person);

            Experience.BindModelToControls(person);
            Education.BindModelToControls(person);
            Document.BindModelToControls(person);
            Achievement.BindModelToControls(person);

            if (Experience.ItemsCount == 0 || Education.ItemsCount == 0)
                EducationTab.IsSelected = true;
        }

        private void ShowWarnings(Person person)
        {
            var isExperienceSet = TCandidateLanguageProficiencySearch
                .SelectFirst(x => x.UserIdentifier == User.Identifier && x.OrganizationIdentifier == Organization.Identifier) != null;

            if (!isExperienceSet)
                StatusAlert.AddMessage(AlertType.Warning, "Please fill all mandatory fields in the <strong>Profile</strong> tab and the <strong>Education and Experience</strong> tab");

            if (person.JobsApproved == null)
                StatusAlert.AddMessage(AlertType.Warning, "This account is still <strong>pending approval</strong> by Admins.<br>Once approved this will be reflected in company searches for your profile");
        }

        private void Save()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(User.Identifier, Organization.Identifier);
            var homeAddress = person.GetAddress(AddressType.Home.ToString());

            JobInterest.BindControlsToModel(person, homeAddress);

            PersonStore.Update(person);

            LanguageAbility.SaveProficiencies(person);
            Document.SaveUploads(person);

            BindModelToControls();

            StatusAlert.AddMessage(AlertType.Success, "Successfully saved profile data");
        }
    }
}