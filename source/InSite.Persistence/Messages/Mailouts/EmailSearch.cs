using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Messages.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class EmailSearch
    {
        private class ReadHelper : ReadHelper<VMailout>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VMailout>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.XMailouts.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

        }

        public static T[] Distinct<T>(Expression<Func<VMailout, T>> binder, Expression<Func<VMailout, bool>> filter, string modelSort = null)
        {
            return ReadHelper.Instance.Distinct(binder, filter, modelSort);
        }

        public static int Count(EmailFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public static VMailout SelectFirstOrDefault(EmailFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).FirstOrDefault();
        }

        public static List<VMailout> Select(EmailFilter filter, params Expression<Func<VMailout, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderByDescending(x => x.MailoutScheduled)
                    .ThenBy(x => x.RecipientListTo)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public static List<VMailout> Select(Guid identifier)
        {
            var sortExpression = nameof(VMailout.RecipientListTo);
            using (var db = CreateContext())
                return CreateQuery(identifier, db)
                    .OrderBy(sortExpression)
                    .ToList();
        }

        private static IQueryable<VMailout> CreateQuery(Guid identifier, InternalDbContext db)
        {
            return identifier != Guid.Empty ? FilterQuery(db.XMailouts.AsQueryable(), identifier) : db.XMailouts.AsQueryable();
        }

        private static IQueryable<VMailout> CreateQuery(EmailFilter filter, InternalDbContext db)
        {
            return filter != null ? FilterQuery(db.XMailouts.AsQueryable(), filter, db) : db.XMailouts.AsQueryable();
        }

        private static IQueryable<VMailout> FilterQuery(IQueryable<VMailout> query, Guid identifier)
        {

            if (identifier != Guid.Empty)
                query = query.Where(x => x.MailoutIdentifier == identifier);

            return query;
        }

        private static IQueryable<VMailout> FilterQuery(IQueryable<VMailout> query, EmailFilter filter, InternalDbContext db)
        {
            if (filter.OrganizationIdentifier != Guid.Empty)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.EmailIdentifier != Guid.Empty)
                query = query.Where(x => x.MailoutIdentifier == filter.EmailIdentifier);

            if (filter.Organizations != null)
            {
                var organizations = new List<Guid>();

                foreach (var organization in filter.Organizations)
                    organizations.Add(organization.OrganizationIdentifier);

                query = query.Where(x => organizations.Contains(x.OrganizationIdentifier));
            }

            if (filter.DeliveredSince.HasValue)
                query = query.Where(x => x.MailoutScheduled >= filter.DeliveredSince.Value);

            if (filter.DeliveredBefore.HasValue)
                query = query.Where(x => x.MailoutScheduled < filter.DeliveredBefore.Value);

            if (filter.EmailBody.IsNotEmpty())
                query = query.Where(x => x.ContentBodyHtml.Contains(filter.EmailBody));

            if (filter.SenderName.IsNotEmpty())
                query = query.Where(x => x.SenderName.Contains(filter.SenderName));

            if (filter.ToName.IsNotEmpty())
                query = query.Where(x => db.Users.Any(y => x.RecipientListTo.Contains(y.Email) && y.FullName.Contains(filter.ToName)));

            if (filter.SenderEmail.IsNotEmpty())
                query = query.Where(x => x.SenderEmail.Contains(filter.SenderEmail));

            if (filter.EmailTo.IsNotEmpty())
                query = query.Where(x => x.RecipientListTo.Contains(filter.EmailTo));

            if (filter.EmailSubject.IsNotEmpty())
                query = query.Where(x => x.ContentSubject.Contains(filter.EmailSubject));

            if (filter.StatusCode.IsNotEmpty())
                query = query.Where(x => x.MailoutStatusCode.Contains(filter.StatusCode));

            if (filter.StatusMessage.IsNotEmpty())
                query = query.Where(x => x.MailoutStatus.Contains(filter.StatusMessage));

            if (filter.DeliverySuccessful.HasValue)
            {
                if (filter.DeliverySuccessful.Value)
                    query = query.Where(x => x.MailoutStatus == "Completed" || x.MailoutStatus == "Succeeded");
                else
                    query = query.Where(x => x.MailoutStatus == "Error" || x.MailoutStatus == "Failed");
            }

            return query;
        }

        private static InternalDbContext CreateContext() => new InternalDbContext(false);
    }
}
