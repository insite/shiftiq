using System;
using System.Linq;

using InSite.Application;
using InSite.Application.Memberships.Write;

using Shift.Common;

namespace InSite.Persistence
{
    public static class MembershipStore
    {
        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public static Guid Save(Membership membership, bool modifyFunction = false, bool modifyEffective = false, bool modifyExpiry = false)
        {
            if (_commander == null)
                throw new Exception("The MembershipStore must be initialized before it is used.");

            Guid membershipId;

            var script = new MembershipScript();

            using (var db = new InternalDbContext())
            {
                var existing = db.Memberships
                    .SingleOrDefault(x => x.GroupIdentifier == membership.GroupIdentifier &&
                                          x.UserIdentifier == membership.UserIdentifier);

                if (existing != null)
                {
                    membershipId = existing.MembershipIdentifier;

                    UpdateExistingMembership(membership, existing, modifyFunction, modifyEffective, modifyExpiry, script, db);
                }
                else
                {
                    membershipId = InsertNewMembership(membership, modifyFunction, modifyEffective, modifyExpiry, script, db);
                }
            }

            script.Execute(_commander);

            return membershipId;
        }

        private static void UpdateExistingMembership(
            Membership updated, Membership stored,
            bool modifyFunction, bool modifyEffective, bool modifyExpiry,
            MembershipScript script,
            InternalDbContext db
            )
        {
            var id = stored.MembershipIdentifier;

            if (modifyFunction && updated.MembershipType != stored.MembershipType)
                script.Add(new ModifyMembershipFunction(id, updated.MembershipType));

            var effective = stored.Assigned;

            if (modifyEffective && updated.Assigned != stored.Assigned && updated.Assigned != DateTimeOffset.MinValue)
            {
                effective = updated.Assigned;
                script.Add(new ModifyMembershipEffective(id, effective));
            }

            if (modifyExpiry)
            {
                var expiry = CalculateExpiry(db, updated.GroupIdentifier, effective, updated.MembershipExpiry);
                if (expiry != stored.MembershipExpiry && expiry != DateTimeOffset.MinValue)
                    script.Add(new ModifyMembershipExpiry(id, expiry));
            }
        }

        private static Guid InsertNewMembership(
            Membership inserted,
            bool modifyFunction, bool modifyEffective, bool modifyExpiry,
            MembershipScript script,
            InternalDbContext db
            )
        {
            var user = inserted.UserIdentifier;
            var group = inserted.GroupIdentifier;

            if (inserted.OrganizationIdentifier == null || inserted.OrganizationIdentifier == Guid.Empty)
                inserted.OrganizationIdentifier = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == inserted.GroupIdentifier)?.OrganizationIdentifier;

            if (inserted.Assigned == DateTimeOffset.MinValue)
                inserted.Assigned = DateTimeOffset.UtcNow;

            var effective = inserted.Assigned;
            var function = inserted.MembershipType;

            var identifier = CreateMembershipIdentifier(db, user, group);

            if (identifier.isNew)
                script.Add(new StartMembership(identifier.id, user, group, function, effective));
            else
                script.Add(new ResumeMembership(identifier.id, user, group, function, effective));

            if (modifyFunction)
                script.Add(new ModifyMembershipFunction(identifier.id, inserted.MembershipType));

            if (modifyEffective && effective != DateTimeOffset.MinValue)
                script.Add(new ModifyMembershipEffective(identifier.id, effective));

            if (modifyExpiry)
            {
                var expiry = CalculateExpiry(db, group, effective, inserted.MembershipExpiry);
                if (expiry.HasValue && expiry != DateTimeOffset.MinValue)
                    script.Add(new ModifyMembershipExpiry(identifier.id, expiry));
            }

            return identifier.id;
        }

        private static DateTimeOffset? CalculateExpiry(InternalDbContext db, Guid groupId, DateTimeOffset started, DateTimeOffset? membershipExpiry)
        {
            var group = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == groupId);
            if (group == null)
                return membershipExpiry;

            if (group.LifetimeQuantity == null || group.LifetimeUnit == null)
                return membershipExpiry;

            if (group.LifetimeUnit == "Year")
                return started.AddYears(group.LifetimeQuantity.Value);

            if (group.LifetimeUnit == "Month")
                return started.AddMonths(group.LifetimeQuantity.Value);

            if (group.LifetimeUnit == "Week")
                return started.AddDays(7 * group.LifetimeQuantity.Value);

            if (group.LifetimeUnit == "Day")
                return started.AddDays(group.LifetimeQuantity.Value);

            return membershipExpiry;
        }

        /// <remarks>
        /// If a membership for this user and group existed in the past, and was deleted, then reuse its membership 
        /// identifier. Otherwise create a new identifier.
        /// </remarks>
        private static (Guid id, bool isNew) CreateMembershipIdentifier(InternalDbContext db, Guid user, Guid group)
        {
            var deletion = db.QMembershipDeletions.FirstOrDefault(x => x.UserIdentifier == user && x.GroupIdentifier == group);

            if (deletion != null)
                return (deletion.MembershipIdentifier, false);

            return (UniqueIdentifier.Create(), true);
        }

        public static void Delete(Membership membership)
        {
            if (_commander == null)
                throw new Exception("The MembershipStore must be initialized before it is used.");

            if (membership == null)
                return;

            var entity = MembershipSearch.Select(membership.GroupIdentifier, membership.UserIdentifier);
            if (entity == null)
                return;

            var id = entity.MembershipIdentifier;

            _commander.Send(new EndMembership(id));
        }
    }
}