using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Domain.Messages;
using InSite.Domain.Users;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Utilities
{
    public static class PersonHelper
    {
        public static UserAccountWelcomed CreateWelcomeMessage(Guid organizationId, Guid userId, bool increaseSendWelcomeEmailCounter = true)
        {
            var organization = OrganizationSearch.Select(organizationId)
                ?? throw ApplicationError.Create("Organization not found: {0}", organizationId);

            var user = UserSearch.Select(userId)
                ?? throw ApplicationError.Create("User not found: {0}", userId);

            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId)
                ?? throw ApplicationError.Create("Person not found: OrganizationID={0}, UserID={1}", organizationId, userId);

            var password = user.IsDefaultPassword() && user.DefaultPasswordExpired > DateTimeOffset.UtcNow
                ? user.DefaultPassword : null;

            if (increaseSendWelcomeEmailCounter)
            {
                if (person.WelcomeEmailsSentToUser == null)
                    person.WelcomeEmailsSentToUser = 1;
                else
                    person.WelcomeEmailsSentToUser++;

                PersonStore.Update(person);
            }

            return new UserAccountWelcomed
            {
                TenantIdentifier = organizationId,
                TenantCode = organization.OrganizationCode,
                TenantName = organization.CompanyName,

                UserAccessGranted = person.UserAccessGranted,
                UserEmail = user.Email,
                UserFirstName = user.FirstName,
                UserIdentifier = user.UserIdentifier,
                UserPassword = password,
                UserPasswordHash = user.UserPasswordHash
            };
        }

        public static void SendWelcomeMessage(Guid organizationId, Guid userId, Guid[] ccUserIds = null, bool increaseSendWelcomeEmailCounter = true)
        {
            var notification = CreateWelcomeMessage(organizationId, userId, increaseSendWelcomeEmailCounter);
            var alert = new AlertUserAccountWelcomed
            {
                TenantCode = notification.TenantCode,
                TenantIdentifier = notification.TenantIdentifier,
                TenantName = notification.TenantName,
                UserAccessGranted = notification.UserAccessGranted,
                UserEmail = notification.UserEmail,
                UserFirstName = notification.UserFirstName,
                UserIdentifier = notification.UserIdentifier,
                Url = HttpRequestHelper.CurrentRootUrl,
                WelcomeMsgDictionary = ServiceLocator.CoreProcessManager.GetWelcomeMsgPasswordInfo
                        (notification.UserPassword, notification.UserPasswordHash)
            };

            ServiceLocator.AlertMailer.Send(organizationId, userId, alert, ccUserIds);
        }

        public static void SendAccountCreated(Guid organizationId, string organizationName, QUser user, QPerson person)
        {
            ServiceLocator.AlertMailer.Send(organizationId, user.UserIdentifier, new AlertUserAccountCreated
            {
                TenantIdentifier = organizationId,
                Tenant = organizationName,
                Name = user.FullName.IfNullOrEmpty($"{user.FirstName} {user.LastName}"),
                Email = user.Email,
                Phone = person.Phone.IfNullOrEmpty("Phone not specified"),
                City = person.HomeAddress?.City?.IfNullOrEmpty("City not specified"),
                Province = person.HomeAddress?.Province?.IfNullOrEmpty("Province not specified"),
                CompanyTitle = CurrentSessionState.Identity.Organization.CompanyName.EmptyIfNull()
            });
        }
    }
}