using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class QMembershipReasonSearch : IMembershipReasonSearch
    {
        private InternalDbContext CreateContext() => new InternalDbContext(false);

        public QMembershipReason Select(Guid reasonId, params Expression<Func<QMembershipReason, object>>[] includes)
        {
            using (var db = CreateContext())
                return db.QMembershipReasons
                    .ApplyIncludes(includes)
                    .FirstOrDefault(x => x.ReasonIdentifier == reasonId);
        }

        public List<QMembershipReason> Select(QMembershipReasonFilter filter, params Expression<Func<QMembershipReason, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db, includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(nameof(QMembershipReason.ReasonIdentifier));

                return query.ApplyPaging(filter).ToList();
            }
        }

        public int Count(QMembershipReasonFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public bool Exists(QMembershipReasonFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Any();
        }

        public List<ReferralGridDataItem> SelectForReferralGrid(QMembershipReasonFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(nameof(QMembershipReason.ReasonIdentifier));

                return query
                    .ApplyPaging(filter)
                    .Select(x => new ReferralGridDataItem
                    {
                        ReasonIdentifier = x.ReasonIdentifier,
                        ReasonType = x.ReasonType,
                        ReasonSubtype = x.ReasonSubtype,
                        PersonOccupation = x.PersonOccupation,
                        ReasonEffective = x.ReasonEffective,
                        ReasonExpiry = x.ReasonExpiry,

                        MembershipIdentifier = x.MembershipIdentifier,
                        UserIdentifier = x.Membership.UserIdentifier,
                        GroupIdentifier = x.Membership.GroupIdentifier,
                        GroupName = x.Membership.Group.GroupName,
                        GroupParentIdentifier = x.Membership.Group.Parent.GroupIdentifier,
                        GroupParentType = x.Membership.Group.Parent.GroupType,
                        GroupParentName = x.Membership.Group.Parent.GroupName,
                        GroupOrganizationIdentifier = x.Membership.Group.OrganizationIdentifier,
                        GroupOrganizationCode = x.Membership.Group.Organization.OrganizationCode,
                    })
                    .ToList();
            }
        }

        private static IQueryable<QMembershipReason> CreateQuery(QMembershipReasonFilter filter, InternalDbContext db, params Expression<Func<QMembershipReason, object>>[] includes)
        {
            var query = db.QMembershipReasons.ApplyIncludes(includes);

            if (filter.MembershipIdentifier.HasValue)
                query = query.Where(x => x.MembershipIdentifier == filter.MembershipIdentifier.Value);

            if (filter.GroupOrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.GroupOrganizationIdentifiers.Contains(x.Membership.Group.OrganizationIdentifier));

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.Membership.GroupIdentifier == filter.GroupIdentifier.Value);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.Membership.UserIdentifier == filter.UserIdentifier.Value);

            if (filter.GroupName.IsNotEmpty())
                query = query.Where(x => x.Membership.Group.GroupName.Contains(filter.GroupName));

            if (filter.ReasonType.IsNotEmpty())
                query = query.Where(x => x.ReasonType == filter.ReasonType);

            if (filter.ReasonSubtype.IsNotEmpty())
                query = query.Where(x => x.ReasonSubtype.Contains(filter.ReasonSubtype));

            if (filter.ReasonEffectiveSince.HasValue)
                query = query.Where(x => x.ReasonEffective >= filter.ReasonEffectiveSince.Value);

            if (filter.ReasonEffectiveBefore.HasValue)
                query = query.Where(x => x.ReasonEffective < filter.ReasonEffectiveBefore.Value);

            if (filter.ReasonExpirySince.HasValue)
                query = query.Where(x => x.ReasonExpiry >= filter.ReasonExpirySince.Value);

            if (filter.ReasonExpiryBefore.HasValue)
                query = query.Where(x => x.ReasonExpiry < filter.ReasonExpiryBefore.Value);

            if (filter.PersonOccupation.IsNotEmpty())
                query = query.Where(x => x.PersonOccupation.Contains(filter.PersonOccupation));

            if (filter.PersonCode.IsNotEmpty() && filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Membership.User.Persons.Any(y => y.OrganizationIdentifier == filter.OrganizationIdentifier.Value && y.PersonCode.Contains(filter.PersonCode)));

            if (filter.ReasonCreatedBy.HasValue)
                query = query.Where(x => x.CreatedBy == filter.ReasonCreatedBy.Value);

            return query;
        }
    }
}
