using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Credentials.Write;
using InSite.Application.Memberships.Write;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.People.Write;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

using OrganizationIdentifiers = Shift.Constant.OrganizationIdentifiers;

namespace InSite.Admin.Contacts.People.Utilities
{
    public class ArchiveHelper
    {
        private string _actorName;

        private readonly bool _deactivateContact;
        private readonly bool _removeFromGroups;
        private readonly bool _disableNotifications;
        private readonly bool _revokeAccess;
        private readonly bool _removeFromOrganizations;

        public ArchiveHelper(string actorName,
            bool deactivateContact, bool removeFromGroups, bool disableNotifications, bool revokeAccess, bool removeFromOrganizations)
        {
            _actorName = actorName;

            _deactivateContact = deactivateContact;
            _removeFromGroups = removeFromGroups;
            _disableNotifications = disableNotifications;
            _revokeAccess = revokeAccess;
            _removeFromOrganizations = removeFromOrganizations;
        }

        public ArchiveHelper(string actorName)
        {
            _actorName = actorName;
        }

        public void Archive(Guid personId, Guid userId, Guid organizationId, Guid? employerId)
        {
            if (_deactivateContact)
            {
                ServiceLocator.SendCommand(new ArchivePerson(personId, DateTimeOffset.Now));
            }

            if (_removeFromGroups)
            {
                var memberships = MembershipSearch.Bind(m => m, m => m.UserIdentifier == userId && m.Group.OrganizationIdentifier == organizationId);
                foreach (var membership in memberships)
                    ServiceLocator.SendCommand(new EndMembership(membership.MembershipIdentifier));

                if (employerId.HasValue)
                    ServiceLocator.SendCommand(new ModifyPersonFieldGuid(personId, PersonField.EmployerGroupIdentifier, null));
            }

            if (_disableNotifications)
            {
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(personId, PersonField.EmailEnabled, false));
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(personId, PersonField.EmailAlternateEnabled, false));
            }

            if (_revokeAccess)
            {
                ServiceLocator.SendCommand(new RevokePersonAccess(personId, DateTimeOffset.Now, _actorName));
            }

            if (_removeFromOrganizations)
            {
                CmdsArchive(userId, organizationId);
            }
        }

        public void Unarchive(Guid personId)
        {
            ServiceLocator.SendCommand(new UnarchivePerson(personId, DateTimeOffset.Now));
        }

        public void CmdsArchive(Guid userId, Guid organizationId)
        {
            var filter = new QSubscriberUserFilter { SubscriberIdentifier = userId };
            var subscriptions = ServiceLocator.MessageSearch.GetSubscriberUsers(filter);
            foreach (var subscription in subscriptions)
                ServiceLocator.SendCommand(new RemoveMessageSubscriber(subscription.MessageIdentifier, subscription.UserIdentifier, false));

            ServiceLocator.SendCommand(new ArchiveCmdsUser(userId));

            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId, x => x.HomeAddress);
            var home = person?.HomeAddress;

            var organizations = PersonSearch.GetOrganizationIds(userId);
            foreach (var organization in organizations)
                PersonStore.Delete(userId, organization);

            var memberships = MembershipSearch.Bind(m => m, m => m.UserIdentifier == userId && m.Group.GroupType == "Department");
            foreach (var membership in memberships)
                MembershipStore.Delete(membership);

            UserConnectionStore.DeleteDownstream(userId);
            UserConnectionStore.DeleteUpstream(userId);

            VoidCredentials(userId);

            MoveUserToArchiveOrganization(userId, home);
        }

        public void CmdsArchive(QUser user, Guid organizationId)
        {
            CmdsArchive(user.UserIdentifier, organizationId);

            user.SetDefaultPassword(Default.CmdsPassword);

            user.Email = user.UserIdentifier.ToString().Substring(0, 8) + "@keyeracmds.com";
            user.UtcArchived = DateTimeOffset.UtcNow;
            user.UtcUnarchived = null;

            UserStore.Update(user, null);
        }

        public void CmdsUnarchive(QUser user)
        {
            user.UtcArchived = null;
            user.UtcUnarchived = DateTimeOffset.UtcNow;
            user.Email = user.Email;
            user.UserPasswordExpired = DateTimeOffset.UtcNow;

            user.SetDefaultPassword(Default.CmdsPassword);

            UserStore.Update(user, null);
            PersonStore.Delete(user.UserIdentifier, OrganizationIdentifiers.Archive);
        }

        private void VoidCredentials(Guid user)
        {
            var credentials = VCmdsCredentialSearch.Select(x =>
                x.UserIdentifier == user
                && x.Achievement.Visibility != AccountScopes.Enterprise);

            foreach (var credential in credentials)
                ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));
        }

        private void MoveUserToArchiveOrganization(Guid user, QPersonAddress home)
        {
            var departments = DepartmentSearch.SelectCompanyDepartments(OrganizationIdentifiers.Archive);
            var department = departments.Find(x => string.Equals(x.DepartmentName, "Unassigned", StringComparison.OrdinalIgnoreCase)).DepartmentIdentifier;
            MembershipStore.Save(MembershipFactory.Create(user, department, OrganizationIdentifiers.Archive, "Department"));

            var person = PersonFactory.Create(user, OrganizationIdentifiers.Archive, null, false, null);
            PersonStore.Insert(person);

            if (home == null)
                return;

            var address = home.ToModel();
            address.Identifier = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new ModifyPersonAddress(person.PersonIdentifier, AddressType.Home, address));
        }
    }
}