using System;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class PersonFactory
    {
        public static QPerson Create(Guid user, Guid organization, string code, bool grantAccess, string accessGrantedBy)
        {
            var person = new QPerson
            {
                IsLearner = true,
                OrganizationIdentifier = organization,
                PersonCode = code,
                PersonIdentifier = UniqueIdentifier.Create(),
                UserIdentifier = user,

                BillingAddress = new QPersonAddress(),
                HomeAddress = new QPersonAddress(),
                ShippingAddress = new QPersonAddress(),
                WorkAddress = new QPersonAddress()
            };

            if (grantAccess)
            {
                person.UserAccessGranted = DateTimeOffset.Now;
                person.UserAccessGrantedBy = accessGrantedBy;
            }

            return person;
        }
    }
}