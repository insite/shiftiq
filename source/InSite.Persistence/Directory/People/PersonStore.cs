using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.Contacts.Write;
using InSite.Application.People.Write;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Persistence
{
    public static class PersonStore
    {
        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public static void Insert(QPerson person)
        {
            if (person.PersonIdentifier == Guid.Empty)
                person.PersonIdentifier = UniqueIdentifier.Create();

            if (person.UserIdentifier == Guid.Empty)
                throw new ArgumentNullException("UserIdentifier");

            if (person.OrganizationIdentifier == Guid.Empty)
                throw new ArgumentNullException("OrganizationIdentifier");

            if (!person.Gender.HasValue())
                person.Gender = "Unspecified";

            IEnumerable<Guid> groups;

            using (var db = new InternalDbContext())
            {
                if (db.QPersons.Where(x => x.OrganizationIdentifier == person.OrganizationIdentifier && x.UserIdentifier == person.UserIdentifier).Any())
                    return;

                groups = db.Groups.Where(x => x.OrganizationIdentifier == person.OrganizationIdentifier && x.AddNewUsersAutomatically && !x.OnlyOperatorCanAddUser)
                    .Select(x => x.GroupIdentifier)
                    .ToList();
            }

            var commands = PersonCommandCreator.Create(null, person);
            SendCommands(commands);

            foreach (var group in groups)
                MembershipStore.Save(MembershipFactory.Create(person.UserIdentifier, group, person.OrganizationIdentifier));
        }

        public static void Update(QPerson person)
        {
            QPerson oldPerson;
            using (var db = new InternalDbContext())
                oldPerson = db.QPersons.Where(x => x.PersonIdentifier == person.PersonIdentifier).FirstOrDefault();

            var commands = PersonCommandCreator.Create(oldPerson, person);
            SendCommands(commands);
        }

        public static void Delete(Guid userId, Guid organizationId)
        {
            IEnumerable<Membership> memberships;

            bool hideUser;
            Guid personId;

            using (var db = new InternalDbContext())
            {
                var person = db.Persons
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .FirstOrDefault();

                if (person == null)
                    return;

                personId = person.PersonIdentifier;

                memberships = db.Memberships
                    .Where(x => x.UserIdentifier == userId && x.Group.OrganizationIdentifier == organizationId)
                    .ToList();

                var profiles = db.DepartmentProfileUsers.Where(x => x.UserIdentifier == userId
                    && ((db.Departments.FirstOrDefault(y => y.DepartmentIdentifier == x.DepartmentIdentifier).OrganizationIdentifier == organizationId)
                        || (db.Standards.FirstOrDefault(y => y.StandardIdentifier == x.ProfileStandardIdentifier).OrganizationIdentifier == organizationId))
                );

                if (profiles.Any())
                    db.DepartmentProfileUsers.RemoveRange(profiles);

                hideUser = db.QUsers.Any(x => x.UserIdentifier == userId)
                    && !db.QPersons.Any(x => x.UserIdentifier == userId && x.OrganizationIdentifier != organizationId);

                db.SaveChanges();
            }

            _commander.Send(new DeletePerson(personId));

            if (hideUser)
            {
                _commander.Send(new ModifyUserFieldText(userId, UserField.Email, userId.ToString()));
                _commander.Send(new ModifyUserDefaultPassword(userId, null, null));
            }

            foreach (var membership in memberships)
                MembershipStore.Delete(membership);
        }

        private static void SendCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
                _commander.Send(command);
        }
    }
}