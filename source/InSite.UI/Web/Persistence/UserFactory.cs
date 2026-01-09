using System;

using InSite.Application.Contacts.Read;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Data
{
    public class UserFactory
    {
        public QUser User { get; set; }
        public QPerson Person { get; set; }

        /// <summary>
        /// Adds a new user to the database, following the multi-tenancy rules defined 
        /// </summary>
        public void RegisterUser(string email, Guid organization, string firstName, string lastName, string password, Guid? employer, string phone, string language, bool defaultMFA, string middleName = null)
        {
            var (isNewUser, isNewPerson) = RegisterUserAndPerson(email, organization, firstName, lastName, password, middleName);

            if (isNewUser || isNewPerson)
            {
                Person.MemberStartDate = DateTime.Today;
                Person.EmployerGroupIdentifier = employer;
                Person.Phone = phone;
                Person.Language = Language.CodeExists(language) ? language : null;
            }

            if (isNewUser)
            {
                User.MultiFactorAuthentication = defaultMFA;
                UserStore.Insert(User, Person);
                return;
            }

            UserStore.Update(User, OrganizationSearch.GetPersonFullNamePolicy(organization));

            if (isNewPerson)
            {
                Person.UserIdentifier = User.UserIdentifier;
                PersonStore.Insert(Person);
            }
            else
            {
                PersonStore.Update(Person);
            }
        }

        private (bool IsNewUser, bool IsNewPerson) RegisterUserAndPerson(string email, Guid organization, string firstName, string lastName, string password, string middleName = null)
        {
            User = ServiceLocator.UserSearch.GetUserByEmail(email);

            if (User == null)
            {
                User = Create();
                Person = CreatePerson(organization);

                User.FirstName = firstName;
                User.LastName = lastName;
                User.Email = email;

                if (middleName != null) 
                    User.MiddleName = middleName;

                if (password != null)
                    User.SetPassword(password);

                Person.EmailEnabled = true;

                return (true, false);
            }

            Person = ServiceLocator.PersonSearch.GetPerson(User.UserIdentifier, organization, x => x.HomeAddress, x => x.WorkAddress);
            if (Person != null)
                return (false, false);

            Person = CreatePerson(organization);

            return (false, true);
        }

        public static QUser Create()
        {
            var identity = CurrentSessionState.Identity;

            var organization = identity?.Organization;
            if (organization == null)
                organization = OrganizationSearch.Select(CookieTokenModule.Current.OrganizationCode);

            return new QUser
            {
                TimeZone = organization.TimeZone.Id,
                MultiFactorAuthentication = organization.Toolkits.Contacts?.DefaultMFA ?? false
            };
        }

        public static QPerson CreatePerson(Guid organizationIdentifier, Guid? creator = null)
        {
            var identity = CurrentSessionState.Identity;

            if (creator == null)
                creator = identity?.User?.Identifier ?? Shift.Constant.UserIdentifiers.Someone;

            var created = DateTimeOffset.UtcNow;

            var person = new QPerson
            {
                OrganizationIdentifier = organizationIdentifier,
                IsLearner = true,
                Created = created,
                Modified = created,
                CreatedBy = creator.Value,
                ModifiedBy = creator.Value,
                Gender = "Unspecified",
                FirstLanguage = "English",
            };

            person.BillingAddress = new QPersonAddress();
            person.HomeAddress = new QPersonAddress();
            person.ShippingAddress = new QPersonAddress();
            person.WorkAddress = new QPersonAddress();

            var autoincrement = identity?.Organization.Toolkits.Accounts?.PersonCodeAutoincrement;

            if (autoincrement?.Enabled == true)
                person.PersonCode = GenerateUniquePersonCode(organizationIdentifier, autoincrement);

            return person;
        }

        private static string GenerateUniquePersonCode(
            Guid organizationId,
            PersonCodeAutoincrementSettings autoincrement)
        {
            int attempt = 0;
            const int maxAttempts = 20;

            string generated;

            do
            {
                if (attempt++ > maxAttempts)
                    throw new InvalidOperationException("Unable to generate a unique PersonCode after multiple attempts.");

                var number = Sequence.Increment(
                    organizationId,
                    SequenceType.PersonCode,
                    autoincrement.StartNumber
                );

                generated = autoincrement.Format.IsNotEmpty()
                    ? autoincrement.Format.Format(number)
                    : number.ToString();

            }
            while (ServiceLocator.PersonSearch.IsPersonExist(organizationId, generated, null));

            return generated;
        }
    }
}