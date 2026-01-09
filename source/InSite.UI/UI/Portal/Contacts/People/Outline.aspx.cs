using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Contacts.People
{
    public partial class Outline : PortalBasePage, IHasTitle
    {
        private Guid LearnerIdentifier => Guid.TryParse(Request.QueryString["learner"], out Guid id) ? id : Guid.Empty;

        private QUser _user;
        private QPerson _person;

        public string GetTitle()
        {
            LoadContact();
            return _person?.FullName ?? _user?.FullName;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/management/dashboard/home");
            PortalMaster.RenderHelpContent(null);

            PageHelper.AutoBindHeader(this);

            if (Organization.OrganizationIdentifier == OrganizationIdentifiers.SkillsCheck)
                PortalMaster.HideBreadcrumbsOnly();

            if (Identity.IsAuthenticated)
                OverrideHomeLink("/ui/portal/management/dashboard/home");
            else
                OverrideHomeLink("/ui/portal/billing/catalog");

            Open();
        }

        private void Open()
        {
            LoadContact();

            if (_user == null)
                HttpResponseHelper.Redirect("/ui/portal/contacts/people/search");

            Detail.SetModel(_user, _person);

            var achievementCount = Achievements.LoadData(Organization.Identifier, LearnerIdentifier);
            AchievementSection.SetTitle("Achievements", achievementCount);

            var registrationCount = Registrations.LoadData(Organization.Identifier, LearnerIdentifier);
            RegistrationSection.SetTitle("Registrations", registrationCount);

            var recordCount = Records.LoadData(Organization.Identifier, LearnerIdentifier);
            RecordSection.SetTitle("Records", recordCount);
        }

        private void LoadContact()
        {
            if (_user != null)
                return;

            if (ServiceLocator.UserSearch.GetConnection(User.Identifier, LearnerIdentifier) == null)
                return;

            _person = ServiceLocator.PersonSearch.GetPerson(LearnerIdentifier, Organization.Identifier, x => x.User);
            _user = _person?.User;
        }
    }
}