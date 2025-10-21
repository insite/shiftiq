using System;

using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio
{
    public partial class View : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            BindModelToControls();
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
    }
}
