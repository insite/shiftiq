using System;
using System.Text.RegularExpressions;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Application.People.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Contacts;

namespace InSite.Persistence
{
    public static class GroupHelper
    {
        public static void Delete(
            ICommander commander,
            IGroupSearch groupSearch,
            IRegistrationSearch registrationSearch,
            IPersonSearch personSearch,
            Guid groupIdentifier
            )
        {
            var registrationCustomerIds = registrationSearch
                .GetRegistrationIdentifiers(new QRegistrationFilter { RegistrationCustomerIdentifier = groupIdentifier });

            foreach (var registrationId in registrationCustomerIds)
                commander.Send(new AssignCustomer(registrationId, null));

            var registrationEmployerIds = registrationSearch
                .GetRegistrationIdentifiers(new QRegistrationFilter { RegistrationEmployerIdentifier = groupIdentifier });

            foreach (var registrationId in registrationEmployerIds)
                commander.Send(new AssignEmployer(registrationId, null));

            var children = groupSearch.GetGroups(new QGroupFilter { ParentGroupIdentifier = groupIdentifier });
            foreach (var child in children)
                commander.Send(new ChangeGroupParent(child.GroupIdentifier, null));

            var connectedChildren = groupSearch.GetGroups(new QGroupFilter { ConnectParentGroupIdentifier = groupIdentifier });
            foreach (var child in connectedChildren)
                commander.Send(new DisconnectGroup(child.GroupIdentifier, groupIdentifier));

            var employees = personSearch.GetPersonsByEmployer(groupIdentifier);
            foreach (var employee in employees)
                commander.Send(new ModifyPersonFieldGuid(employee.PersonIdentifier, PersonField.EmployerGroupIdentifier, null));

            commander.Send(new DeleteGroup(groupIdentifier, null));
        }

        private static readonly Regex NonAlphaNumericRegex = new Regex("[^0-9a-zA-Z ]");
        public static string CleanGroupLabel(string groupLabel)
        {
            return !string.IsNullOrEmpty(groupLabel)
                ? NonAlphaNumericRegex.Replace(groupLabel, "")
                : groupLabel;
        }
    }
}
