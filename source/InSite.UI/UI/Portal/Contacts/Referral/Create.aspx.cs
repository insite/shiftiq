using System;
using System.Web.UI;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Memberships.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Contacts.Referral
{
    public partial class Create : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OccupationIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.ListFilter.StandardLabel = "Occupation List";
            OccupationIdentifier.RefreshData();

            ReferrerIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            ReferrerIdentifier.ListFilter.CollectionName = CollectionName.Contacts_Settings_Referrers_Name;
            ReferrerIdentifier.RefreshData();

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Identity.IsGranted("Portal/Contacts", PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/portal/contacts/referral/search");

            PageHelper.AutoBindHeader(Page);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var currentPerson = ServiceLocator.PersonSearch.GetPerson(CurrentSessionState.Identity.User.Identifier, Organization.OrganizationIdentifier);

            var user = SaveUser();
            var person = SavePerson(user, currentPerson);

            AddMembershipReferral(person.UserIdentifier, currentPerson.EmployerGroupIdentifier);

            HttpResponseHelper.Redirect($"/ui/portal/contacts/referral/outline?learner={user.UserIdentifier}");
        }

        private QUser SaveUser()
        {
            var email = PersonEmail.Text;
            var user = ServiceLocator.UserSearch.GetUserByEmail(email);

            if (user == null)
            {
                user = UserFactory.Create();
                user.MultiFactorAuthentication = Organization.Toolkits.Contacts?.DefaultMFA ?? false;
                GetUserInputValues(user);
                UserStore.Insert(user, Organization.Toolkits.Contacts?.FullNamePolicy);
            }
            else
            {
                GetUserInputValues(user);
                UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));
            }

            return user;
        }

        private QPerson SavePerson(QUser user, QPerson currentPerson)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier);
            if (person != null)
                return person;

            person = UserFactory.CreatePerson(Organization.Identifier);
            GetPersonInputValues(person);

            person.UserIdentifier = user.UserIdentifier;
            person.EmailEnabled = true;

            if (currentPerson?.EmployerGroupIdentifier != null)
                person.EmployerGroupIdentifier = currentPerson.EmployerGroupIdentifier.Value;

            var autoGroupJoinId = Organization.Toolkits.Accounts?.AutomaticGroupJoin;
            if (autoGroupJoinId.HasValue)
            {
                person.UserAccessGranted = DateTimeOffset.UtcNow;
                person.UserAccessGrantedBy = User.FullName;
            }

            PersonStore.Insert(person);

            AddMembership(person.UserIdentifier, autoGroupJoinId);
            AddMembership(person.UserIdentifier, person.EmployerGroupIdentifier);

            if (person.UserAccessGranted.HasValue)
            {
                PersonHelper.SendAccountCreated(Organization.OrganizationIdentifier, Organization.LegalName, user, person);
                PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);
            }

            return person;
        }

        private void GetUserInputValues(QUser user)
        {
            if (user.FirstName.IsEmpty())
                user.FirstName = PersonFirstName.Text;

            if (user.MiddleName.IsEmpty())
                user.MiddleName = PersonMiddleName.Text;

            if (user.LastName.IsEmpty())
                user.LastName = PersonLastName.Text;

            if (user.Email.IsEmpty())
                user.Email = PersonEmail.Text;

            if (user.TimeZone.IsEmpty())
                user.TimeZone = Organization.TimeZone.Id;
        }

        private void GetPersonInputValues(QPerson person)
        {
            person.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;
            person.Birthdate = PersonBirthdate.Value;
            person.Phone = PersonPhone.Text;
            person.HomeAddress = GetAddress();
            person.Referrer = ReferrerIdentifier.GetSelectedOption()?.Text;
        }

        private void AddMembership(Guid userId, Guid? group)
        {
            if (group.HasValue && MembershipPermissionHelper.CanModifyMembership(group.Value))
            {
                MembershipHelper.Save(new Membership
                {
                    UserIdentifier = userId,
                    GroupIdentifier = group.Value,
                    Assigned = DateTimeOffset.UtcNow
                });
            }
        }

        private void AddMembershipReferral(Guid userId, Guid? groupId)
        {
            if (!groupId.HasValue)
                return;

            var membershipId = ServiceLocator.MembershipSearch.GetMembershipId(userId, groupId.Value);
            if (!membershipId.HasValue)
            {
                AddMembership(userId, groupId);
                membershipId = ServiceLocator.MembershipSearch.GetMembershipId(userId, groupId.Value);
            }

            var reasonId = UniqueIdentifier.Create();
            var reasonEffective = DateTimeOffset.UtcNow;
            var reasonExpiry = reasonEffective.AddYears(1);

            ServiceLocator.SendCommand(new AddMembershipReason(
                membershipId.Value,
                reasonId,
                "Referral",
                ReferrerIdentifier.GetSelectedOption()?.Text,
                reasonEffective,
                reasonExpiry,
                OccupationIdentifier.GetSelectedOption()?.Text));
        }

        private QPersonAddress GetAddress()
        {
            return new QPersonAddress()
            {
                Street1 = Street1.Text,
                Street2 = Street2.Text,
                City = City.Text,
                Province = Province.Text,
                PostalCode = PostalCode.Text,
                Country = Country.Text
            };
        }
    }
}