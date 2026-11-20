using System;
using System.Collections.Generic;
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

        public static Guid Save(Membership membership, bool modifyFunction = false, bool modifyEffective = false, bool modifyExpiry = false, bool matchOrganization = false)
        {
            if (_commander == null)
                throw new InvalidOperationException("The MembershipStore must be initialized before it is used.");

            Guid membershipId;

            var script = new MembershipScript();

            using (var db = new InternalDbContext())
            {
                var query = db.Memberships.AsQueryable();

                if (matchOrganization)
                    query = query.Where(x => x.OrganizationIdentifier == membership.OrganizationIdentifier);

                var matchIdentifier = membership.MembershipIdentifier != Guid.Empty;

                query = matchIdentifier
                    ? query.Where(x => x.MembershipIdentifier == membership.MembershipIdentifier)
                    : query.Where(x => x.GroupIdentifier == membership.GroupIdentifier && x.UserIdentifier == membership.UserIdentifier);

                var existing = matchIdentifier || matchOrganization
                    ? query.SingleOrDefault()
                    : SelectExisting(query.ToList(), membership, db);

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

        /// <remarks>
        /// When a user holds memberships in the same group for more than one organization, update
        /// the one in the organization this membership would be inserted into. If there is none,
        /// then the caller inserts it there.
        /// </remarks>
        private static Membership SelectExisting(List<Membership> candidates, Membership membership, InternalDbContext db)
        {
            if (candidates.Count <= 1)
                return candidates.FirstOrDefault();

            var organization = membership.OrganizationIdentifier;

            if (organization == null || organization == Guid.Empty)
                organization = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == membership.GroupIdentifier)?.OrganizationIdentifier;

            return candidates.SingleOrDefault(x => x.OrganizationIdentifier == organization);
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

            var identifier = CreateMembershipIdentifier(db, user, group, inserted.OrganizationIdentifier);

            if (identifier.isNew)
            {
                var start = new StartMembership(identifier.id, user, group, function, effective);

                if (inserted.OrganizationIdentifier.HasValue)
                    start.OriginOrganization = inserted.OrganizationIdentifier.Value;

                script.Add(start);
            }
            else
            {
                // The projection takes the membership's organization from the change, so without
                // this the resumed membership would belong to the organization of whoever is
                // signed in.
                var resume = new ResumeMembership(identifier.id, user, group, function, effective);

                if (inserted.OrganizationIdentifier.HasValue)
                    resume.OriginOrganization = inserted.OrganizationIdentifier.Value;

                script.Add(resume);
            }

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
        /// If a membership for this user, group, and organization existed in the past, and was
        /// deleted, then reuse its membership identifier. Otherwise create a new identifier. The
        /// organization matters because a user can hold one membership in a partition-wide group
        /// per organization (TEC-854), and a deleted membership might have been resumed since, so
        /// its identifier might be in use already.
        /// </remarks>
        private static (Guid id, bool isNew) CreateMembershipIdentifier(InternalDbContext db, Guid user, Guid group, Guid? organization)
        {
            var deletion = db.QMembershipDeletions
                .Where(x => x.UserIdentifier == user && x.GroupIdentifier == group && x.OrganizationIdentifier == organization)
                .Where(x => !db.QMemberships.Any(y => y.MembershipIdentifier == x.MembershipIdentifier))
                .OrderByDescending(x => x.DeletionWhen)
                .FirstOrDefault();

            if (deletion != null)
                return (deletion.MembershipIdentifier, false);

            return (UniqueIdentifier.Create(), true);
        }

        public static void Delete(Membership membership)
        {
            if (_commander == null)
                throw new InvalidOperationException("The MembershipStore must be initialized before it is used.");

            if (membership == null)
                return;

            if (membership.MembershipIdentifier != Guid.Empty)
            {
                Delete(membership.MembershipIdentifier);
                return;
            }

            var entity = MembershipSearch.Select(membership.GroupIdentifier, membership.UserIdentifier, membership.OrganizationIdentifier);
            if (entity == null)
                return;

            _commander.Send(new EndMembership(entity.MembershipIdentifier));
        }

        public static void Delete(Guid membershipId)
        {
            if (_commander == null)
                throw new InvalidOperationException("The MembershipStore must be initialized before it is used.");

            if (!MembershipSearch.Exists(x => x.MembershipIdentifier == membershipId))
                return;

            _commander.Send(new EndMembership(membershipId));
        }
    }
}
