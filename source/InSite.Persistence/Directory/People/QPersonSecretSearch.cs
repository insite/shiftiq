using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Persistence.Foundation;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class QPersonSecretSearch : IPersonSecretSearch
    {
        private static InternalDbContext CreateContext()
        {
            var db = new InternalDbContext();
            db.Configuration.LazyLoadingEnabled = false;
            return db;
        }

        public QPersonSecret GetSecret(Guid secretId)
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SecretIdentifier == secretId);
            }
        }

        public QPersonSecret GetSecret(Guid secretId, params Expression<Func<QPersonSecret, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .FirstOrDefault(x => x.SecretIdentifier == secretId);
            }
        }

        public QPersonSecret GetByPerson(Guid personId, string name)
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets
                    .AsNoTracking()
                    .FirstOrDefault(x => x.PersonIdentifier == personId && x.SecretName == name);
            }
        }

        public QPersonSecret GetBySecretValue(string secret)
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SecretValue == secret);
            }
        }

        public QPersonSecret GetBySecretValue(string secret, params Expression<Func<QPersonSecret, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .FirstOrDefault(x => x.SecretValue == secret);
            }
        }

        public int Count()
        {
            using (var db = CreateContext())
            {
                return db.QPersonSecrets.Count();
            }
        }

        public int Count(QPersonSecretFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = ApplyFilter(db.QPersonSecrets.AsNoTracking(), filter);
                return query.Count();
            }
        }

        public T[] Bind<T>(Expression<Func<QPersonSecret, T>> binder, QPersonSecretFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = ApplyFilter(db.QPersonSecrets.AsNoTracking(), filter);
                return query.Select(binder).ApplyPaging(filter.Paging).ToArray();
            }
        }

        public T[] Bind<T>(Expression<Func<QPersonSecret, T>> binder, QPersonSecretFilter filter, params Expression<Func<QPersonSecret, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = ApplyFilter(db.QPersonSecrets.AsNoTracking(), filter).ApplyIncludes(includes);

                return query
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .Select(binder)
                    .ToArray();
            }
        }

        private IQueryable<QPersonSecret> ApplyFilter(IQueryable<QPersonSecret> query, QPersonSecretFilter filter)
        {
            if (filter == null) return query;

            if (filter.UserIdentifier.HasValue)
            {
                var personIds = GetPersonIdsFromUserId(filter.UserIdentifier.Value);
                query = query.Where(x => personIds.Contains(x.PersonIdentifier));
            }

            if (!string.IsNullOrEmpty(filter.OrganizationCode))
            {
                var orgPersonIds = GetPersonIdsFromOrganizationCode(filter.OrganizationCode);
                query = query.Where(x => orgPersonIds.Contains(x.PersonIdentifier));
            }

            if (filter.TokenIssuedSince.HasValue)
                query = query.Where(x => x.SecretExpiry >= filter.TokenIssuedSince.Value);

            if (filter.TokenIssuedBefore.HasValue)
                query = query.Where(x => x.SecretExpiry <= filter.TokenIssuedBefore.Value);

            if (filter.TokenExpiredSince.HasValue)
                query = query.Where(x => x.SecretExpiry >= filter.TokenExpiredSince.Value);

            if (filter.TokenExpiredBefore.HasValue)
                query = query.Where(x => x.SecretExpiry <= filter.TokenExpiredBefore.Value);

            return query;
        }

        private List<Guid> GetPersonIdsFromOrganizationCode(string organizationCode)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                         .Where(p => p.Organization.OrganizationCode == organizationCode)
                         .Select(p => p.PersonIdentifier)
                         .ToList();
            }
        }

        private List<Guid> GetPersonIdsFromUserId(Guid userId)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                         .Where(p => p.UserIdentifier == userId)
                         .Select(p => p.PersonIdentifier)
                         .ToList();
            }
        }
    }
}
