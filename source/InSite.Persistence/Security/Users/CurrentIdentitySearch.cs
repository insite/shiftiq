using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;

using Shift.Common;

using PersonModel = InSite.Domain.Foundations.Person;
using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Persistence
{
    public class CurrentIdentitySearch
    {
        public CurrentIdentity Get(
            string userName,
            string organizationCode,
            string language,
            string impersonatorLoginName,
            string impersonatorOrganizationCode,
            List<QPerson> people,
            IGroupSearch groupSearch,
            IPersonSearch personSearch
            )
        {
            var model = new IdentityModel(organizationCode);

            var organizationId = model.Organization.Identifier;

            var user = GetUser(userName, organizationId);

            if (user != null)
            {
                var userId = user.Identifier;

                LoadImpersonator(model, impersonatorLoginName, impersonatorOrganizationCode, organizationId);

                LoadOrganizations(model, userId, userName);

                LoadGroupsAndPermissions(model, userId, organizationId, model.Organization.ParentOrganizationIdentifier, groupSearch);

                LoadPerson(model, userId, people, personSearch);
            }

            return CurrentIdentity.Create
                (
                    language,
                    model.Organization, model.Organizations,
                    user,
                    model.Impersonator,
                    model.Person, model.Persons,
                    model.Groups,
                    model.Claims
                );
        }

        private void LoadPerson(IdentityModel model, Guid userId, List<QPerson> people, IPersonSearch personSearch)
        {
            var organizationId = model.Organization.Identifier;

            if (people == null)
                people = personSearch.GetPersons(new QPersonFilter { UserIdentifier = userId });

            var person = people.First(x => x.OrganizationIdentifier == organizationId);

            model.Person = PersonAdapter.CreatePersonPacket(person);

            foreach (var p in people)
                model.Persons.Add(PersonAdapter.CreatePersonPacket(p));
        }

        private void LoadGroupsAndPermissions(IdentityModel model, Guid userId, Guid organizationId, Guid? parentOrganizationId, IGroupSearch groupSearch)
        {
            var roles = MembershipSearch.Select(
                    x => x.UserIdentifier == userId,
                    x => x.User);

            var upstreamOrganizations = new List<Guid>
                {
                    model.Organization.Identifier
                };

            if (parentOrganizationId.HasValue && parentOrganizationId != Guid.Empty)
            {
                upstreamOrganizations.Add(parentOrganizationId.Value);
                var parent = OrganizationSearch.Select(parentOrganizationId.Value);
                if (parent.ParentOrganizationIdentifier.HasValue && parent.ParentOrganizationIdentifier != Guid.Empty)
                    upstreamOrganizations.Add(parent.ParentOrganizationIdentifier.Value);
            }

            foreach (var role in roles)
            {
                var group = groupSearch.GetGroup(role.GroupIdentifier);
                if (group == null)
                    continue;

                var permissions = TGroupPermissionSearch.Select(x => x.GroupIdentifier == group.GroupIdentifier);

                bool enabled = upstreamOrganizations.Any(o => o == group.OrganizationIdentifier);

                if (!enabled)
                    continue;

                model.Groups.Add(GroupAdapter.CreateGroupPacket(group));

                foreach (var permission in permissions)
                {
                    var action = TActionSearch.Get(permission.ObjectIdentifier);
                    model.Claims.Add(permission.ObjectIdentifier,
                        action?.ActionType, action?.ActionUrl,
                        permission.AllowExecute,
                        permission.AllowRead, permission.AllowWrite, permission.AllowCreate, permission.AllowDelete,
                        permission.AllowAdministrate, permission.AllowConfigure,
                        permission.AllowTrialAccess);
                }
            }
        }

        private void LoadOrganizations(IdentityModel model, Guid user, string userName)
        {
            model.Organizations = OrganizationHelper.SelectOrganizationsAccessibleToUser(user);

            if (model.Organizations.Count == 0)
                throw new MissingPersonException(userName);

            else if (!model.Organizations.Contains(model.Organization.Identifier))
                model.Organization = model.Organizations.First();
        }

        private void LoadImpersonator(IdentityModel model, string impersonatorLoginName, string impersonatorOrganizationCode, Guid organization)
        {
            if (impersonatorLoginName == null || impersonatorOrganizationCode == null)
                return;

            var user = UserSearch.SelectWebContact(impersonatorLoginName, organization, false)
                ?? throw new MissingPersonException(impersonatorLoginName, organization.ToString());

            var impersonator = new Impersonator
            {
                User = user,
                Organization = OrganizationSearch.Select(impersonatorOrganizationCode)
            };
            impersonator.Organizations = OrganizationHelper.SelectOrganizationsAccessibleToUser(impersonator.User.Identifier);
            model.Impersonator = impersonator;
        }

        private UserModel GetUser(string userName, Guid organization)
        {
            return userName.HasValue()
                ? UserSearch.SelectWebContact(userName, organization, false)
                : null;
        }

        private class IdentityModel
        {
            internal IdentityModel(string organizationCode)
            {
                Organization = GetOrganization(organizationCode);
                Groups = new GroupList();
                Person = null;
                Persons = new PersonList();
                Organizations = new OrganizationList();
                Claims = new ClaimList();
                Impersonator = null;
            }

            internal OrganizationState Organization { get; set; }
            internal GroupList Groups { get; set; }
            internal PersonModel Person { get; set; }
            internal PersonList Persons { get; set; }
            internal OrganizationList Organizations { get; set; }
            internal ClaimList Claims { get; set; }
            internal Impersonator Impersonator { get; set; }

            private OrganizationState GetOrganization(string organization)
            {
                var org = OrganizationSearch.Select(organization);

                if (org == null)
                    throw new OrganizationNotFoundException(organization);

                return org;
            }
        }
    }
}