using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class QMembershipSearch : IMembershipSearch
    {
        private InternalDbContext CreateContext()
            => new InternalDbContext(false) { EnablePrepareToSaveChanges = false };

        public QMembership Select(Guid membership)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships.FirstOrDefault(x => x.MembershipIdentifier == membership);
            }
        }

        public QMembership Select(Guid user, Guid group)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships.FirstOrDefault(x => x.UserIdentifier == user && x.GroupIdentifier == group);
            }
        }

        public QMembershipDeletion SelectDeletion(Guid membership)
        {
            using (var db = CreateContext())
            {
                return db.QMembershipDeletions.FirstOrDefault(x => x.MembershipIdentifier == membership);
            }
        }

        public bool Exists(Guid user, Guid group)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships.Any(x => x.UserIdentifier == user && x.GroupIdentifier == group);
            }
        }

        public Guid? GetMembershipId(Guid user, Guid group)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships
                    .Where(x => x.UserIdentifier == user && x.GroupIdentifier == group)
                    .Select(x => (Guid?)x.MembershipIdentifier)
                    .FirstOrDefault();
            }
        }

        public List<Guid> GetUserAllMembershipIds(Guid user)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships
                    .Where(x => x.UserIdentifier == user)
                    .Select(x => x.MembershipIdentifier)
                    .Concat(
                        db.QMembershipDeletions
                            .Where(x => x.UserIdentifier == user)
                            .Select(x => x.MembershipIdentifier))
                    .Distinct()
                    .ToList();
            }
        }

        public List<Guid> GetGroupAllMembershipIds(Guid group)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships
                    .Where(x => x.GroupIdentifier == group)
                    .Select(x => x.MembershipIdentifier)
                    .Concat(
                        db.QMembershipDeletions
                            .Where(x => x.GroupIdentifier == group)
                            .Select(x => x.MembershipIdentifier))
                    .Distinct()
                    .ToList();
            }
        }

        public List<QMembership> SelectExpired(DateTimeOffset expireDate)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships.Where(x => x.MembershipExpiry <= expireDate).ToList();
            }
        }

        public List<QMembership> SelectExpired(Guid groupId, DateTimeOffset nowDate, int lifetimeDays)
        {
            using (var db = CreateContext())
            {
                return db.QMemberships
                    .Where(x => x.GroupIdentifier == groupId
                             && SqlFunctions.DateDiff("DAY", x.MembershipEffective, nowDate) > lifetimeDays)
                    .ToList();
            }
        }

        public List<QMembership> Select(QMembershipFilter filter, params Expression<Func<QMembership, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyIncludes(includes)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        private IQueryable<QMembership> CreateQuery(QMembershipFilter filter, InternalDbContext db)
        {
            var query = db.QMemberships.AsNoTracking().AsQueryable();

            if (filter.GroupIdentifiers.IsNotEmpty())
            {
                if (filter.GroupIdentifiers.Length == 1)
                {
                    var value = filter.GroupIdentifiers[0];
                    query = query.Where(x => x.GroupIdentifier == value);
                }
                else
                    query = query.Where(x => filter.GroupIdentifiers.Contains(x.GroupIdentifier));
            }

            if (filter.UserIdentifiers.IsNotEmpty())
            {
                if (filter.UserIdentifiers.Length == 1)
                {
                    var value = filter.UserIdentifiers[0];
                    query = query.Where(x => x.UserIdentifier == value);
                }
                else
                    query = query.Where(x => filter.UserIdentifiers.Contains(x.UserIdentifier));
            }

            return query;
        }
    }
}