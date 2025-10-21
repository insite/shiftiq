using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    /// <remarks>
    /// Implements the insert, update, and delete operations for Individual records imported from Direct Access.
    /// </remarks>
    public class DirectAccessStore : IDirectAccessStore
    {
        private static readonly object locker = new object();

        private readonly IDirectAccessSearch _search;
        private readonly IUserSearch _userSearch;
        private readonly IPersonSearch _personSearch;

        public DirectAccessStore(IDirectAccessSearch search, IUserSearch userSearch, IPersonSearch personSearch)
        {
            _search = search;
            _userSearch = userSearch;
            _personSearch = personSearch;
        }

        /// <remarks>
        /// Given an Individual from Direct Access, ensure the InSite database contains a matching User and Person (i.e. connection to the SkilledTradesBC
        /// organization account). Update the user's name, email, birthdate, and phone numbers. There are four (4) possible scenarios:
        ///    1. New Tradeworker with an Unused Email: Insert User, Insert Person
        ///    2. Existing Tradeworker with an Unused Email: Update User (with new email address)
        ///    3. New Tradeworker with a Used Email: Update User (no change to email address), Insert Person
        ///    4. Existing Tradeworker with a Used Email: Update User (special case needed to determine if Tradeworker.Email = MatchingUser.Email)
        ///
        /// This function might be invoked by multiple concurrent SkilledTradesBC administrators. It is thread-safe.
        /// </remarks>
        public void Save(Individual individual)
        {
            lock (locker)
            {
                // Ensure the incoming data from Direct Access is clean.
                individual.PrepareForSerialization();

                // Save a copy of the Individual exactly as it arrived from the Direct Access database.
                SaveFromDirectAccess(individual);

                // Insert or update the corresponding User and Person in the InSite database.
                SaveToShift(individual);
            }
        }

        private void SaveFromDirectAccess(Individual individual)
        {
            using (var db = new InternalDbContext())
            {
                var isNew = !db.Individuals.Any(x => x.IndividualKey == individual.IndividualKey);

                if (isNew)
                {
                    db.Individuals.Add(individual);
                }
                else
                {
                    individual.Refreshed = DateTimeOffset.UtcNow;
                    db.Individuals.Attach(individual);
                    db.Entry(individual).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        private void SaveToShift(Individual individual)
        {
            var personCode = individual.IndividualKey.ToString().ToLower();
            var person = PersonSearch.Select(OrganizationIdentifiers.SkilledTradesBC, personCode, x => x.User);
            var user = UserSearch.SelectByEmail(individual.Email);

            var scenario = DetermineScenario(person, user);

            switch (scenario)
            {
                case 1:
                    AddNewUser(individual, personCode);
                    break;

                case 2:
                    UpdateExistingUser(person.UserIdentifier, individual, individual.Email, false);
                    break;

                case 3:
                    ConnectExistingUserToOrganization(user.UserIdentifier, personCode);
                    UpdateExistingUser(user.UserIdentifier, individual, individual.Email, false);
                    break;

                case 4:
                    var email = _search.GetUniqueEmail(individual.Email, user.UserIdentifier, person.User.Email, person.UserIdentifier);
                    var emailIsSystemGenerated = !StringHelper.Equals(email, person.User.Email);
                    UpdateExistingUser(person.UserIdentifier, individual, email, emailIsSystemGenerated);
                    break;
            }
        }

        private int DetermineScenario(Person person, User user)
        {
            if (person == null && user == null)
                return 1;

            if (person != null && user == null)
                return 2;

            if (person == null && user != null)
                return 3;

            else // person != null && user != null 
                return 4;
        }

        private void AddNewUser(Individual individual, string code)
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifiers.SkilledTradesBC)
                ?? throw new ApplicationException("SkillTradesBC org is not found");

            var user = new QUser
            {
                UserIdentifier = UniqueIdentifier.Create(),
                TimeZone = TimeZones.Pacific.Id,
                MultiFactorAuthentication = organization.Toolkits.Contacts?.DefaultMFA ?? false
            };

            var person = new QPerson
            {
                OrganizationIdentifier = OrganizationIdentifiers.SkilledTradesBC,
                IsLearner = true,
                PersonCode = code,
                EmailEnabled = true
            };

            SetProperties(user, person, individual, individual.Email);

            UserStore.Insert(user, person);
        }

        private void ConnectExistingUserToOrganization(Guid userId, string code)
        {
            PersonStore.Insert(PersonFactory.Create(userId, OrganizationIdentifiers.SkilledTradesBC, code, false, null));
        }

        private void UpdateExistingUser(Guid userId, Individual individual, string email, bool emailIsSystemGenerated)
        {
            var person = _personSearch.GetPerson(userId, OrganizationIdentifiers.SkilledTradesBC);
            var user = _userSearch.GetUser(userId);
            var fullNamePolicy = OrganizationSearch.GetPersonFullNamePolicy(person.OrganizationIdentifier);

            SetProperties(user, person, individual, email);

            person.EmailEnabled = EmailAddress.IsValidAddress(user.Email) && !emailIsSystemGenerated;
            person.EmailAlternateEnabled = EmailAddress.IsValidAddress(user.EmailAlternate) && emailIsSystemGenerated;

            UserStore.Update(user, fullNamePolicy);
            PersonStore.Update(person);
        }

        private void SetProperties(QUser user, QPerson person, Individual individual, string email)
        {
            user.Email = email;

            if (!StringHelper.Equals(user.Email, individual.Email))
                user.EmailAlternate = individual.Email;

            user.FirstName = individual.FirstName;
            user.LastName = individual.LastName;
            user.PhoneMobile = Phone.Format(individual.Mobile);

            person.Birthdate = individual.Birthdate;
            person.Phone = Phone.Format(individual.Phone);
        }
    }
}
