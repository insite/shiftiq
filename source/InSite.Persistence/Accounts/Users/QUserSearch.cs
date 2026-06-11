using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class QUserSearch : IUserSearch
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false, false);
        }

        public bool IsUserExist(Guid userId)
        {
            using (var db = CreateContext())
                return db.QUsers.Where(x => x.UserIdentifier == userId).Any();
        }

        public bool IsUserExist(string email)
        {
            using (var db = CreateContext())
                return db.QUsers.Where(x => x.Email == email).Any();
        }

        public bool? IsOrphan(string email)
        {
            using (var db = CreateContext())
                return db.QUsers.Where(x => x.Email == email).Select(x => (bool?)x.Persons.Any()).FirstOrDefault();
        }

        public QUser GetUser(Guid userId)
        {
            using (var db = CreateContext())
                return db.QUsers.Where(x => x.UserIdentifier == userId).FirstOrDefault();
        }

        public QUser GetUserByEmail(string email)
        {
            using (var db = CreateContext())
                return db.QUsers.Where(x => x.Email == email).FirstOrDefault();
        }

        public List<QUserConnection> GetConnections(Guid fromUserId)
        {
            using (var db = CreateContext())
                return db.QUserConnections.Where(x => x.FromUserIdentifier == fromUserId).ToList();
        }

        public QUserConnection GetConnection(Guid fromUserId, Guid toUserId)
        {
            using (var db = CreateContext())
                return db.QUserConnections.FirstOrDefault(x => x.FromUserIdentifier == fromUserId && x.ToUserIdentifier == toUserId);
        }

        public int CountConnections(QUserConnectionFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QUserConnection> GetConnections(QUserConnectionFilter filter, params Expression<Func<QUserConnection, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyIncludes(includes)
                    .OrderBy(x => x.FromUser.FullName)
                    .ThenBy(x => x.ToUser.FullName)
                    .ToList();
            }
        }

        private static IQueryable<QUserConnection> CreateQuery(QUserConnectionFilter filter, InternalDbContext db)
        {
            var query = db.QUserConnections.AsQueryable();

            if (filter == null)
                return query;

            if (filter.FromUserOrganizationId.HasValue)
                query = query.Where(x => x.FromUser.Persons.Any(person => person.OrganizationIdentifier == filter.FromUserOrganizationId.Value));

            if (filter.FromUserId.HasValue)
                query = query.Where(x => x.FromUserIdentifier == filter.FromUserId.Value);

            if (filter.ToUserOrganizationId.HasValue)
                query = query.Where(x => x.ToUser.Persons.Any(person => person.OrganizationIdentifier == filter.ToUserOrganizationId.Value));

            if (filter.ToUserId.HasValue)
                query = query.Where(x => x.ToUserIdentifier == filter.ToUserId.Value);

            if (filter.FromUserName.HasValue())
                query = query.Where(x => x.FromUser.FullName.Contains(filter.FromUserName));

            if (filter.FromUserName.HasValue())
                query = query.Where(x => x.FromUser.FullName.Contains(filter.FromUserName));

            if (filter.ToUserName.HasValue())
                query = query.Where(x => x.ToUser.FullName.Contains(filter.ToUserName));

            if (filter.IsManager)
                query = query.Where(x => x.IsManager == filter.IsManager);

            if (filter.IsValidator)
                query = query.Where(x => x.IsValidator == filter.IsValidator);

            if (filter.IsSupervisor)
                query = query.Where(x => x.IsSupervisor == filter.IsSupervisor);

            return query;
        }

    }
}
