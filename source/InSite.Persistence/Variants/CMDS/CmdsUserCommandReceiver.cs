using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Exceptions;

using OrganizationIdentifiers = Shift.Constant.OrganizationIdentifiers;

namespace InSite.Persistence.Plugin.CMDS
{
    /// <summary>
    /// Handles the CMDS <see cref="UnarchiveCmdsUser"/> command - the atomic inverse of the CMDS
    /// archive. The person is re-created first, so the user still owns a non-Archive person when
    /// the Archive org person is removed. Without that, PersonStore.Delete would treat the user as
    /// orphaned and blank the email and password. Re-establishing the person via PersonStore.Insert
    /// also re-adds the organization's auto-add groups, restoring the baseline memberships that
    /// archive removed.
    /// </summary>
    /// <remarks>
    /// The Workday reactivation relays this command through the Timeline API and records a 200 as
    /// applied, so the handler refuses anything it cannot carry out rather than returning quietly,
    /// and a repeat of a command that already succeeded changes nothing.
    /// </remarks>
    public class CmdsUserCommandReceiver
    {
        private readonly IUserSearch _users;
        private readonly IPersonSearch _people;

        public CmdsUserCommandReceiver(ICommandQueue commander, IUserSearch users, IPersonSearch people)
        {
            _users = users;
            _people = people;

            commander.Subscribe<UnarchiveCmdsUser>(Handle);
        }

        public void Handle(UnarchiveCmdsUser c)
        {
            var user = _users.GetUser(c.User);
            if (user == null)
                throw new AggregateNotFoundException(typeof(UserAggregate), c.User);

            // The caller binds its later commands to c.Person, so a person already in the
            // organization under another id would leave every one of them aimed at nothing.
            var existing = _people.GetPerson(c.User, c.Organization);
            if (existing != null && existing.PersonIdentifier != c.Person)
                throw new InvalidOperationException(
                    $"User {c.User} already holds person {existing.PersonIdentifier} in organization "
                    + $"{c.Organization}, so the person {c.Person} this command names cannot be created.");

            if (user.UtcArchived == null)
            {
                // Already unarchived: a retried relay or a second click. Running again would reset
                // the password the worker has chosen since.
                if (existing != null)
                    return;

                throw new InvalidOperationException(
                    $"User {c.User} is not archived, so there is nothing to unarchive.");
            }

            // Re-establish the person in the organization before deleting the Archive org person.
            var person = PersonFactory.Create(c.User, c.Organization, null, false, null);
            person.PersonIdentifier = c.Person;
            PersonStore.Insert(person);

            // Clear the archived flag, stamp the unarchive, and reset to the default CMDS password,
            // matching ArchiveHelper.CmdsUnarchive. UserStore.Update diffs against the stored user
            // and emits UnarchiveUser plus the password commands.
            user.UtcArchived = null;
            user.UtcUnarchived = DateTimeOffset.UtcNow;
            user.UserPasswordExpired = DateTimeOffset.UtcNow;
            user.SetDefaultPassword(Shift.Constant.Default.CmdsPassword);
            UserStore.Update(user, null);

            // Remove the Archive org person and every Archive org membership it carries, including
            // the "Unassigned" department placeholder that MoveUserToArchiveOrganization created.
            PersonStore.Delete(c.User, OrganizationIdentifiers.Archive);
        }
    }
}
