using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.Contacts.Write;
using InSite.Application.Users.Write;

using Shift.Common;

namespace InSite.Persistence
{
    public static class UserStore
    {
        internal static InternalDbContext CreateContext(bool proxy = true) => new InternalDbContext(proxy);

        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public static void Delete(Guid userId)
        {
            _commander.Send(new DeleteUser(userId));
        }

        public static void Insert(QUser user, string fullNamePolicy)
        {
            if (user.UserIdentifier == Guid.Empty)
                user.UserIdentifier = UniqueIdentifier.Create();

            if (user.IsNullPassword())
                user.SetDefaultPassword();

            user.UserPasswordChanged = DateTimeOffset.UtcNow;
            user.UserPasswordExpired = DateTimeOffset.UtcNow.AddMonths(6);

            using (var db = CreateContext())
            {
                while (db.QUsers.Any(x => x.Email == user.Email))
                    user.Email = UniqueIdentifier.Create().ToString();
            }

            var userCommands = UserCommandCreator.Create(null, user, fullNamePolicy);
            SendCommands(userCommands);
        }

        public static void Insert(QUser user, QPerson person)
        {
            var fullNamePolicy = OrganizationSearch.GetPersonFullNamePolicy(person.OrganizationIdentifier);

            Insert(user, fullNamePolicy);

            person.UserIdentifier = user.UserIdentifier;

            PersonStore.Insert(person);
        }

        public static void Update(QUser user, string fullNamePolicy)
        {
            if (user.UserIdentifier == Guid.Empty)
                throw new UserNotFoundException($"Unexpected user identifier (0) for {user.Email}");

            QUser oldUser;

            using (var db = CreateContext(false))
            {
                oldUser = db.QUsers.Single(x => x.UserIdentifier == user.UserIdentifier);
            }

            var userCommands = UserCommandCreator.Create(oldUser, user, fullNamePolicy);

            SendCommands(userCommands);
        }

        private static void SendCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
                _commander.Send(command);
        }
    }
}