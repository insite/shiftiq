using System;
using System.Web.UI;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Constant;

namespace InSite.UI.Portal.Contacts.People
{
    public partial class Create : PortalBasePage
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

            if (!Identity.IsGranted("Portal/Contacts", PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/portal/contacts/people/search");

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/management/dashboard/home");
            PortalMaster.RenderHelpContent(null);

            PageHelper.AutoBindHeader(Page);

            if (Organization.OrganizationIdentifier == OrganizationIdentifiers.SkillsCheck)
                PortalMaster.HideBreadcrumbsOnly();

            if (Identity.IsAuthenticated)
                OverrideHomeLink("/ui/portal/management/dashboard/home");
            else
                OverrideHomeLink("/ui/portal/billing/catalog");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var user = SaveUser();

            HttpResponseHelper.Redirect("/ui/portal/contacts/people/search");
        }

        private QUser SaveUser()
        {
            var user = UserFactory.Create();
            var person = UserFactory.CreatePerson(Organization.Identifier);

            GetInputValues(user, person);

            var existUser = ServiceLocator.UserSearch.GetUserByEmail(user.Email);
            if (existUser != null)
            {
                var connection = ServiceLocator.UserSearch.GetConnection(User.Identifier, existUser.UserIdentifier);
                var hasConnection = connection != null;

                if (!hasConnection || !connection.IsSupervisor)
                {
                    var connectCommand = new ConnectUser(User.Identifier, existUser.UserIdentifier, false, false, true, false, DateTimeOffset.UtcNow);

                    if (hasConnection)
                    {
                        connectCommand.IsLeader = connection.IsLeader;
                        connectCommand.IsManager = connection.IsManager;
                        connectCommand.IsValidator = connection.IsValidator;
                        connectCommand.Connected = connection.Connected;
                    }

                    ServiceLocator.SendCommand(connectCommand);
                }

                return existUser;
            }

            var currentPerson = ServiceLocator.PersonSearch.GetPerson(User.UserIdentifier, Organization.OrganizationIdentifier);

            person.EmailEnabled = true;
            person.EmployerGroupIdentifier = currentPerson.EmployerGroupIdentifier;

            var autoGroupJoinId = Organization.Toolkits.Accounts?.AutomaticGroupJoin;
            if (autoGroupJoinId.HasValue)
            {
                person.UserAccessGranted = DateTimeOffset.UtcNow;
                person.UserAccessGrantedBy = User.FullName;
            }

            UserStore.Insert(user, person);

            ServiceLocator.SendCommand(new ConnectUser(User.Identifier, user.UserIdentifier, false, false, true, false, DateTimeOffset.UtcNow));

            AddMembership(person.UserIdentifier, autoGroupJoinId);
            AddMembership(person.UserIdentifier, person.EmployerGroupIdentifier);

            if (person.UserAccessGranted.HasValue)
            {
                PersonHelper.SendAccountCreated(Organization.OrganizationIdentifier, Organization.LegalName, user, person);

                if (!Organization.Toolkits.Portal.NotSendWelcomeMessage)
                    PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);
            }

            return user;
        }

        private void GetInputValues(QUser user, QPerson person)
        {
            user.FirstName = PersonFirstName.Text;
            user.MiddleName = PersonMiddleName.Text;
            user.LastName = PersonLastName.Text;
            user.Email = PersonEmail.Text;
            user.TimeZone = Organization.TimeZone.Id;

            person.Phone = PersonPhone.Text;
            person.HomeAddress = GetAddress();
        }

        private void AddMembership(Guid userId, Guid? group)
        {
            if (!group.HasValue || !MembershipPermissionHelper.CanModifyMembership(group.Value))
                return;

            MembershipHelper.Save(new Membership
            {
                UserIdentifier = userId,
                GroupIdentifier = group.Value,
                Assigned = DateTimeOffset.UtcNow
            });
        }

        private QPersonAddress GetAddress()
        {
            var address = new QPersonAddress()
            {
                Street1 = Street1.Text,
                Street2 = Street2.Text,
                City = City.Text,
                Province = Province.Text,
                PostalCode = PostalCode.Text,
                Country = Country.Text
            };

            return address.IsEmpty() ? null : address;
        }
    }
}