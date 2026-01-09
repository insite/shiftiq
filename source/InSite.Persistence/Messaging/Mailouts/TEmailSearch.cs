using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Messages.Read;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TEmailSearch
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

        public static int Count(Expression<Func<VMailout, bool>> filter)
            => ReadHelper.Instance.Count(filter);

        public static int Count(TEmailFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db).Count();
            }
        }

        public static VMailout Select(Guid id)
        {
            using (var db = new InternalDbContext())
                return db.XMailouts.FirstOrDefault(x => x.MailoutIdentifier == id);
        }

        public static QMailout Get(Guid id)
        {
            using (var db = new InternalDbContext())
                return db.Mailouts.FirstOrDefault(x => x.MailoutIdentifier == id);
        }

        public static SearchResultList Select(TEmailFilter filter)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "MailoutScheduled DESC";

            using (var db = new InternalDbContext())
            {
                var list = CreateQueryByFilter(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();

                return list;
            }
        }

        private static IQueryable<VMailout> CreateQueryByFilter(TEmailFilter filter, InternalDbContext db)
        {
            return db.XMailouts.AsNoTracking().Filter(filter);
        }

        public class MyMessage
        {
            public Guid MailoutIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public string ContentSubject { get; set; }
            public string ContentBodyHtml { get; set; }
            public string ContentBodyText { get; set; }
            public string ContentVariables { get; set; }
            public string SenderName { get; set; }
            public string SenderEmail { get; set; }
            public string RecipientName { get; set; }
            public string RecipientEmail { get; set; }
            public string RecipientVariables { get; set; }
            public DateTimeOffset? MailoutScheduled { get; set; }
            public DateTimeOffset? DeliveryCompleted { get; set; }
        }

        public static MyMessage[] GetMyMessages(Guid user, Guid organization)
        {
            const string query = @"EXEC logs.MyMessages @UserIdentifier, @OrganizationIdentifier";

            object[] sqlParameters =
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@OrganizationIdentifier", organization),
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<MyMessage>(query, sqlParameters)
                    .OrderByDescending(x => x.DeliveryCompleted)
                    .ToArray();
            }
        }

        public static int CountMyMessages(Guid user, Guid organization)
        {
            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<int>(
                        "EXEC logs.MyMessagesCount @UserIdentifier, @OrganizationIdentifier",
                        new[]
                        {
                            new SqlParameter("@UserIdentifier", user),
                            new SqlParameter("@OrganizationIdentifier", organization),
                        })
                    .FirstOrDefault();
        }

        public static MyMessage GetMyMessage(Guid user, Guid mailout)
        {
            const string query = @"EXEC logs.MyMessage @UserIdentifier, @MailoutIdentifier";

            object[] sqlParameters =
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@MailoutIdentifier", mailout),
            };

            using (var db = new InternalDbContext())
            {
                var result = db.Database
                    .SqlQuery<MyMessage>(query, sqlParameters)
                    .FirstOrDefault();

                return result;
            }
        }
    }
}
