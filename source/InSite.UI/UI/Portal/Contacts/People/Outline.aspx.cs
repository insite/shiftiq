using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

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

            PortalMaster.RenderHelpContent(null);

            PageHelper.AutoBindHeader(this);

            DashboardNavigation.Visible = PortalMaster.IsSalesReady;

            if (PortalMaster.IsSalesReady)
            {
                PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/profile");

                PortalMaster.HideBreadcrumbsOnly();

                if (Identity.IsAuthenticated)
                    OverrideHomeLink("/ui/portal/management/dashboard/home");
                else
                    OverrideHomeLink("/ui/portal/billing/catalog");
            }
            else
                PortalMaster.SidebarVisible(false);

            Open();
        }

        private void Open()
        {
            LoadContact();

            if (_user == null)
                HttpResponseHelper.Redirect("/ui/portal/contacts/people/search");

            Detail.SetModel(_user, _person);

            Achievements.LoadData(Organization.Identifier, LearnerIdentifier);
            AchievementSection.SetTitle("Achievements", Achievements.RowCount);

            var registrationCount = Registrations.LoadData(Organization.Identifier, LearnerIdentifier);
            RegistrationSection.SetTitle("Registrations", registrationCount);

            var recordCount = Records.LoadData(Organization.Identifier, LearnerIdentifier);
            RecordSection.SetTitle("Gradebooks", recordCount);

            var programCount = PersonPrograms.LoadData(Organization.Identifier, LearnerIdentifier);
            ProgramSection.SetTitle("Programs", programCount);

            var logbookCount = PersonLogbooks.LoadData(Organization.Identifier, LearnerIdentifier);
            LogbookSection.SetTitle("Logbooks", logbookCount);
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
