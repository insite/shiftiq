using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class MessageSearch : IMessageSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public static MessageSearch Instance { get; } = new MessageSearch();

        private static bool StrictMode { get; set; }

        public static void Init(bool strictMode)
        {
            StrictMode = strictMode;
        }

        private MessageSearch()
        {

        }

        private static T ExecuteOptimisticQuery<T>(Func<InternalDbContext, T> action)
        {
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            using (new TransactionScope(TransactionScopeOption.Required, options))
            {
                using (var db = new InternalDbContext(false, true))
                {
                    return action.Invoke(db);
                }
            }
        }

        #region Messages

        public List<ArchivedFollower> GetArchivedFollowers()
        {
            using (var db = CreateContext())
                return db.ArchivedFollowers.ToList();
        }

        public List<ArchivedSubscriber> GetArchivedSubscribers()
        {
            using (var db = CreateContext())
                return db.ArchivedSubscribers.ToList();
        }

        public bool MessageExists(Guid organization, string messageName)
        {
            using (var db = CreateContext())
            {
                return db.Messages
                    .AsNoTracking()
                    .Any(x => x.OrganizationIdentifier == organization && x.MessageName == messageName);
            }
        }

        public bool MessageExists(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.Messages
                    .AsNoTracking()
                    .Any(x => x.MessageIdentifier == id);
            }
        }

        public List<Counter> CountMessagesByType(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return CreateQueryable(db, filter)
                    .GroupBy(x => x.MessageType)
                    .Select(x => new Counter
                    {
                        Name = x.Key,
                        Value = x.Count()
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        public List<Counter> CountMessageReferences(Guid message)
        {
            using (var db = CreateContext())
            {
                var parameters = new SqlParameter[] {
                        new SqlParameter("@M", message)
                    };
                return db.Database.SqlQuery<Counter>("exec messages.CountMessageReferences @M", parameters).ToList();
            }
        }

        public int CountMessages(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQueryable(db, filter).Count();
            }
        }

        public VMessage GetMessage(Guid id)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.XMessages
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MessageIdentifier == id);
            }
        }

        public VMessage GetMessage(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return CreateQueryable(db, filter).FirstOrDefault();
            }
        }

        public List<VMessage> GetMessages(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var query = CreateQueryable(db, filter)
                    .OrderBy(x => x.OrganizationCode);

                if (filter.OrderBy == "LastModified")
                    query = query.ThenByDescending(x => x.LastChangeTime);
                else
                    query = query.ThenBy(x => x.MessageTitle);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<QMessage> GetQMessages(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var queryable = CreateQQueryable(db, filter);

                if (filter.OrderBy == "LastModified")
                    queryable = queryable.OrderByDescending(x => x.LastChangeTime);
                else
                    queryable = queryable.OrderBy(x => x.MessageTitle);

                return queryable.ToList();
            }
        }

        public QMessage GetQMessage(Guid id)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Messages
                    .AsNoTracking()
                    .Where(x => x.MessageIdentifier == id)
                    .FirstOrDefault();
            }
        }

        public int CountVMessages(MessageFilter filter)
        {
            using (var db = CreateContext())
                return GetVMessages(filter).Count();
        }

        public List<VMessage> GetVMessages(MessageFilter filter, params Expression<Func<VMessage, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var query = CreateQueryable(db, filter)
                    .OrderBy(x => x.OrganizationCode);

                query = filter.OrderBy == "LastModified"
                    ? query.ThenByDescending(x => x.LastChangeTime)
                    : query.OrderBy(x => x.MessageTitle);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<SearchVMessage> GetMessagesWithCount(MessageFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return CreateQueryable(db, filter)
                    .Select(x => new SearchVMessage
                    {
                        MessageIdentifier = x.MessageIdentifier,
                        MessageName = x.MessageName,
                        ContentSubject = x.MessageTitle,
                        OrganizationIdentifier = x.OrganizationIdentifier,
                        MailoutCount = db.Mailouts.Where(y => y.MessageIdentifier == x.MessageIdentifier).Count(),
                        RecipientCount = db.Recipients.Where(y => y.Mailout.MessageIdentifier == x.MessageIdentifier).Count()
                    })
                    .OrderBy(x => x.MessageName)
                    .ThenBy(x => x.ContentSubject)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        #endregion

        #region Links

        public int CountLinks(Guid emailIdentifier)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Links
                    .AsNoTracking()
                    .Count(x => x.MessageIdentifier == emailIdentifier);
            }
        }

        public QLink FindLink(Guid id)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Links
                    .AsNoTracking()
                    .FirstOrDefault(x => x.LinkIdentifier == id);
            }
        }

        public List<QLink> FindLinks(Guid emailIdentifier)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Links
                    .AsNoTracking()
                    .Where(x => x.MessageIdentifier == emailIdentifier).ToList();
            }
        }

        #endregion

        #region Mailouts

        public bool MailoutExists(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.Mailouts
                    .AsNoTracking()
                    .Any(x => x.MailoutIdentifier == id);
            }
        }

        public int CountMailouts(MailoutFilter filter)
        {
            using (var db = CreateContext())
                return CreateQueryable2(db, filter).Count();
        }

        public VMailout FindMailout(Guid mailoutIdentifier)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.XMailouts
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MailoutIdentifier == mailoutIdentifier);
            }
        }

        public List<VMailout> GetMailouts(MailoutFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return CreateQueryable(db, filter)
                    .OrderByDescending(x => x.MailoutScheduled)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public QRecipient GetDeliveryToUser(Guid mailoutId, Guid userId)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Recipients
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MailoutIdentifier == mailoutId && x.UserIdentifier == userId);
            }
        }

        public QRecipient GetDelivery(Guid mailout, Guid recipientId)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Recipients
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MailoutIdentifier == mailout && x.RecipientIdentifier == recipientId);
            }
        }

        public QRecipient GetDelivery(Guid mailout, string recipientAddress)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Recipients
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MailoutIdentifier == mailout && x.UserEmail == recipientAddress);
            }
        }

        public List<QRecipient> GetDeliveries(Guid mailout)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.Recipients
                    .Include(x => x.CarbonCopies)
                    .AsNoTracking()
                    .Where(x => x.MailoutIdentifier == mailout)
                    .OrderBy(x => x.PersonName).ThenBy(x => x.UserEmail)
                    .ToList();
            }
        }

        public List<QRecipient> GetDeliveries(DeliveryFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return CreateQueryable(db, filter)
                    .OrderByDescending(x => x.DeliveryCompleted)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public Dictionary<Guid, string> GetOneRecipientForEachMailout(Guid[] mailouts)
        {
            var result = new Dictionary<Guid, string>();

            string query = @"
select top (1)
       UserEmail         as [Value]
     , MailoutIdentifier as [Key]
from communications.QRecipient
where MailoutIdentifier = @Mailout
";
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                foreach (var mailout in mailouts)
                {
                    var parameters = new SqlParameter[] {
                        new SqlParameter("@Mailout", mailout)
                    };

                    var item = db.Database.SqlQuery<GuidKeyValue>(query, parameters).FirstOrDefault();
                    if (item != null)
                        if (!result.ContainsKey(item.Key))
                            result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        public int CountDeliveries(DeliveryFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQueryable(db, filter).Count();
            }
        }

        public DateTimeOffset? GetLastDeliveryDate(Guid message, Guid user)
        {
            using (var db = CreateContext())
            {
                var delivery = db.Recipients
                    .Where(x => x.Mailout.MessageIdentifier == message && x.RecipientIdentifier == user && x.DeliveryCompleted != null)
                    .OrderByDescending(x => x.DeliveryCompleted)
                    .FirstOrDefault();

                return delivery?.DeliveryCompleted;
            }
        }

        #endregion

        #region Subscribers (Users)

        public int CountSubscriberUsers(Guid message)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(new QSubscriberUserFilter { MessageIdentifier = message }, db).Count();
            }
        }

        public int CountSubscriberUsers(QSubscriberUserFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public ISubscriberPerson GetSubscriberUser(Guid aggregate, Guid contact)
        {
            // The database has two modes for message subscribers. One is "strict" and the other is "loose". The strict
            // mode returns True for the EmailEnabled property only if the user's email address is enabled in the
            // organization that owns the message. The loose mode returns True if the user's email address is enabled
            // in any organization. This is an enterprise-wide setting. Loose mode is enabled in the E03 account only
            // at this time.

            using (var db = CreateContext())
            {
                if (StrictMode)
                    return db.XSubscriberPersons
                        .AsNoTracking()
                        .FirstOrDefault(x => x.MessageIdentifier == aggregate && x.UserIdentifier == contact);

                else
                    return db.XSubscriberUsers
                        .AsNoTracking()
                        .FirstOrDefault(x => x.MessageIdentifier == aggregate && x.UserIdentifier == contact);
            }
        }

        public List<ISubscriberPerson> GetSubscriberUsers(Guid aggregate)
        {
            var filter = new QSubscriberUserFilter
            {
                MessageIdentifier = aggregate
            };
            return GetSubscriberUsers(filter);
        }

        public List<ISubscriberPerson> GetSubscriberUsers(QSubscriberUserFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.UserFullName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<ISubscriberPerson> CreateQuery(QSubscriberUserFilter filter, InternalDbContext db)
        {
            var strict = StrictMode;

            IQueryable<ISubscriberPerson> query;

            if (strict)
                query = db.XSubscriberPersons
                    .AsNoTracking()
                    .AsQueryable();
            else
                query = db.XSubscriberUsers
                    .AsNoTracking()
                    .AsQueryable();

            if (filter == null)
                return query;

            if (filter.MessageIdentifier != Guid.Empty)
                query = query.Where(x => x.MessageIdentifier == filter.MessageIdentifier);

            if (filter.MessageName.HasValue())
                query = query.Where(x => x.MessageName.StartsWith(filter.MessageName));

            if (filter.MessageOrganizationIdentifier.HasValue)
                query = query.Where(x => x.MessageOrganizationIdentifier == filter.MessageOrganizationIdentifier.Value);

            if (filter.SubscriberIdentifier != null)
                query = query.Where(x => x.UserIdentifier == filter.SubscriberIdentifier);

            if (filter.SubscriberIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.SubscriberIdentifiers.Contains(x.UserIdentifier));

            if (filter.SubscriberKeyword.IsNotEmpty())
                query = query.Where(x =>
                       x.UserFullName.Contains(filter.SubscriberKeyword)
                    || x.UserEmail.Contains(filter.SubscriberKeyword)
                );

            return query;
        }

        #endregion

        #region Subscribers (Groups)

        public int CountSubscriberGroups(QSubscriberGroupFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public VSubscriberGroup GetSubscriberGroup(Guid aggregate, Guid contact)
        {
            using (var db = CreateContext())
            {
                return db.VSubscriberGroups
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MessageIdentifier == aggregate && x.GroupIdentifier == contact);
            }
        }

        public List<VSubscriberGroup> GetSubscriberGroups(Guid aggregate)
        {
            var filter = new QSubscriberGroupFilter
            {
                MessageIdentifier = aggregate
            };
            return GetSubscriberGroups(filter);
        }

        public List<VSubscriberGroup> GetSubscriberGroups(QSubscriberGroupFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.GroupName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<VSubscriberGroup> CreateQuery(QSubscriberGroupFilter filter, InternalDbContext db)
        {
            var query = db.VSubscriberGroups
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.MessageIdentifier != Guid.Empty)
                query = query.Where(x => x.MessageIdentifier == filter.MessageIdentifier);

            if (filter.MessageName.HasValue())
                query = query.Where(x => x.MessageName.StartsWith(filter.MessageName));

            if (filter.MessageOrganizationIdentifier.HasValue)
                query = query.Where(x => x.MessageOrganizationIdentifier == filter.MessageOrganizationIdentifier.Value);

            if (filter.SubscriberIdentifier != null)
                query = query.Where(x => x.GroupIdentifier == filter.SubscriberIdentifier);

            if (filter.SubscriberIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.SubscriberIdentifiers.Contains(x.GroupIdentifier));

            if (filter.SubscriberKeyword.IsNotEmpty())
                query = query.Where(x =>
                       x.GroupName.Contains(filter.SubscriberKeyword)
                    || x.GroupCode.Contains(filter.SubscriberKeyword)
                );

            return query;
        }

        #endregion

        #region Subscribers (Users and Groups)

        public List<ISubscriberPerson> GetSubscribers(Guid organization, Guid message, Guid? recipient)
        {
            if (recipient.HasValue)
                return GetSubscribersFromRecipient(organization, message, recipient.Value);

            var list = new Dictionary<Guid, ISubscriberPerson>();

            var users = GetSubscriberUsers(new QSubscriberUserFilter { MessageIdentifier = message });
            foreach (var user in users)
            {
                if (!list.ContainsKey(user.UserIdentifier))
                    list.Add(user.UserIdentifier, user);
            }

            var members = GetSubscribersFromGroups(organization, message);
            foreach (var member in members)
            {
                if (!list.ContainsKey(member.UserIdentifier))
                    list.Add(member.UserIdentifier, member);
            }

            return list.Values.ToList();
        }

        public List<EmailAddress> GetSubscribersEmailAddresses(Guid organization, Guid message, Guid? recipient)
        {
            var subscribers = GetSubscribers(organization, message, recipient);
            var result = new List<EmailAddress>(subscribers.Count);

            foreach (var subscriber in subscribers)
            {
                var address = EmailAddress.GetEnabledEmail(subscriber.UserEmail, subscriber.UserEmailEnabled, subscriber.UserEmailAlternate, subscriber.UserEmailAlternateEnabled);
                if (address.IsNotEmpty())
                    result.Add(new EmailAddress(subscriber.UserIdentifier, address, subscriber.UserFullName, subscriber.PersonCode, subscriber.PersonLanguage));
            }

            return result;
        }

        private List<XSubscriberUser> GetSubscribersFromGroups(Guid organization, Guid message)
        {
            var groups = GetSubscriberGroups(new QSubscriberGroupFilter { MessageIdentifier = message });
            var groupIdentifiers = groups.Select(x => x.GroupIdentifier).ToList();

            return ExecuteOptimisticQuery(db =>
            {
                return db.Memberships
                    .Where(x => groupIdentifiers.Contains(x.GroupIdentifier) && x.Group.OrganizationIdentifier == organization)
                    .Join(db.Persons.Where(x => x.OrganizationIdentifier == organization),
                        member => member.UserIdentifier,
                        person => person.UserIdentifier,
                        (member, person) => new
                        {
                            member.User.UserIdentifier,
                            member.Assigned,
                            member.User.Email,
                            member.User.EmailAlternate,
                            member.User.FirstName,
                            member.User.LastName,
                            member.User.FullName,
                            person.EmailEnabled,
                            person.MarketingEmailEnabled,
                            person.EmailAlternateEnabled,
                            person.PersonCode,
                            person.Language
                        }
                    )
                    .ToList()
                    .Select(x => new XSubscriberUser
                    {
                        MessageIdentifier = message,
                        UserIdentifier = x.UserIdentifier,
                        Subscribed = x.Assigned,
                        UserEmail = x.Email,
                        UserEmailAlternate = x.EmailAlternate,
                        UserFirstName = x.FirstName,
                        UserLastName = x.LastName,
                        UserFullName = x.FullName,
                        UserEmailEnabled = x.EmailEnabled,
                        UserMarketingEmailEnabled = x.MarketingEmailEnabled,
                        UserEmailAlternateEnabled = x.EmailAlternateEnabled,
                        PersonCode = x.PersonCode,
                        PersonLanguage = x.Language
                    })
                    .ToList();
            });
        }

        private static List<ISubscriberPerson> GetSubscribersFromRecipient(Guid organization, Guid message, Guid recipient)
        {
            var list = new List<ISubscriberPerson>();

            var model = PersonCriteria.BindFirst(
                x => new
                {
                    UserIdentifier = x.UserIdentifier,
                    UserEmail = x.User.Email,
                    UserEmailEnabled = x.EmailEnabled,
                    UserMarketingEmailEnabled = x.MarketingEmailEnabled,
                    UserEmailAlternate = x.User.EmailAlternate,
                    UserEmailAlternateEnabled = x.EmailAlternateEnabled,
                    UserFirstName = x.User.FirstName,
                    UserLastName = x.User.LastName,
                    UserFullName = x.User.FullName,
                    PersonCode = x.PersonCode,
                    PersonLanguage = x.Language
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organization,
                    UserIdentifier = recipient
                });

            if (model == null)
                return list;

            list.Add(new XSubscriberUser
            {
                MessageIdentifier = message,
                Subscribed = DateTimeOffset.UtcNow,

                UserIdentifier = model.UserIdentifier,
                UserEmail = model.UserEmail,
                UserEmailEnabled = model.UserEmailEnabled,
                UserMarketingEmailEnabled = model.UserMarketingEmailEnabled,
                UserEmailAlternate = model.UserEmailAlternate,
                UserEmailAlternateEnabled = model.UserEmailAlternateEnabled,
                UserFirstName = model.UserFirstName,
                UserLastName = model.UserLastName,
                UserFullName = model.UserFullName,
                PersonCode = model.PersonCode,
                PersonLanguage = model.PersonLanguage
            });

            return list;
        }

        #endregion

        #region Followers

        public VFollower GetFollower(Guid aggregate, Guid contact, string follower)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.XFollowers
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MessageIdentifier == aggregate && x.SubscriberIdentifier == contact && x.FollowerEmail == follower);
            }
        }

        public VFollower GetFollower(Guid aggregate, Guid contact, Guid follower)
        {
            using (var db = CreateContext())
            {
                return db.XFollowers
                    .AsNoTracking()
                    .FirstOrDefault(x => x.MessageIdentifier == aggregate && x.SubscriberIdentifier == contact && x.FollowerIdentifier == follower);
            }
        }

        public List<VFollower> GetFollowers(Guid aggregate)
        {
            using (var db = CreateContext())
            {
                var query = db.XFollowers
                    .AsNoTracking()
                    .Where(x => x.MessageIdentifier == aggregate)
                    .OrderBy(x => x.FollowerFullName);

                return query.ToList();
            }
        }

        public List<VFollower> GetFollowers(QFollowerFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .OrderBy(x => x.FollowerFullName)
                    .ToList();

                return query.ToList();
            }
        }

        private IQueryable<VFollower> CreateQuery(QFollowerFilter filter, InternalDbContext db)
        {
            var query = db.XFollowers
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.MessageIdentifier.HasValue)
                query = query.Where(x => x.MessageIdentifier == filter.MessageIdentifier);

            if (filter.SubscriberIdentifier.HasValue)
                query = query.Where(x => x.SubscriberIdentifier == filter.SubscriberIdentifier);

            if (filter.FollowerIdentifier.HasValue)
                query = query.Where(x => x.FollowerIdentifier == filter.FollowerIdentifier);

            return query;
        }

        #endregion

        #region Clicks

        public int CountClicks(VClickFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<VClick> GetClicks(VClickFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderByDescending(x => x.Clicked)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<VClick> CreateQuery(VClickFilter filter, InternalDbContext db)
        {
            var query = db.VClicks
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ClickedSince.HasValue)
                query = query.Where(x => x.Clicked >= filter.ClickedSince.Value);

            if (filter.ClickedBefore.HasValue)
                query = query.Where(x => x.Clicked < filter.ClickedBefore.Value);

            if (filter.UserName.HasValue())
                query = query.Where(x => x.UserFullName.Contains(filter.UserName));

            if (filter.UserEmail.HasValue())
                query = query.Where(x => x.UserEmail.Contains(filter.UserEmail));

            if (filter.MessageTitle.HasValue())
                query = query.Where(x => x.MessageTitle.Contains(filter.MessageTitle));

            if (filter.LinkText.HasValue())
                query = query.Where(x => x.LinkText.Contains(filter.LinkText));

            if (filter.LinkUrl.HasValue())
                query = query.Where(x => x.LinkUrl.Contains(filter.LinkUrl));

            if (filter.UserBrowser.HasValue())
                query = query.Where(x => x.UserBrowser.Contains(filter.UserBrowser));

            if (filter.UserHostAddress.HasValue())
                query = query.Where(x => x.UserHostAddress == filter.UserHostAddress);

            return query;
        }

        #endregion

        private IQueryable<VMessage> CreateQueryable(InternalDbContext db, MessageFilter filter)
        {
            var query = db.XMessages.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier.Value);

            if (filter.SenderIdentifier.HasValue)
                query = query.Where(x => x.SenderIdentifier == filter.SenderIdentifier.Value);

            if (filter.IncludeTypes.IsNotEmpty())
                query = query.Where(x => filter.IncludeTypes.Contains(x.MessageType));

            if (filter.ExcludeTypes.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeTypes.Contains(x.MessageType));

            if (filter.HasSender.HasValue)
            {
                if (filter.HasSender.Value)
                    query = query.Where(x => x.SenderEmail != null);
                else
                    query = query.Where(x => x.SenderEmail == null);
            }

            if (filter.IsDisabled.HasValue)
                query = query.Where(x => x.IsDisabled == filter.IsDisabled.Value);

            if (filter.Name.IsNotEmpty())
                query = query.Where(x => x.MessageName.Contains(filter.Name));

            if (filter.Title.IsNotEmpty())
                query = query.Where(x => x.MessageTitle.Contains(filter.Title));

            if (filter.SenderEmail.HasValue())
                query = query.Where(x => x.SenderEmail.Contains(filter.SenderEmail));

            if (filter.SenderNickname.IsNotEmpty())
                query = query.Where(x => x.SenderNickname.Contains(filter.SenderNickname));

            if (filter.SenderName.HasValue())
                query = query.Where(x => x.SenderName.Contains(filter.SenderName));

            if (filter.SenderType.HasValue())
                query = query.Where(x => x.SenderType == filter.SenderType);

            if (filter.SystemMailbox.HasValue())
                query = query.Where(x => x.SystemMailbox.Contains(filter.SystemMailbox));

            if (filter.MessageIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.MessageIdentifiers.Contains(x.MessageIdentifier));

            if (filter.Modified != null && !filter.Modified.IsEmpty)
            {
                if (filter.Modified.Since.HasValue)
                    query = query.Where(x => x.LastChangeTime >= filter.Modified.Since.Value);

                if (filter.Modified.Before.HasValue)
                    query = query.Where(x => x.LastChangeTime < filter.Modified.Before.Value);
            }

            return query;
        }

        private IQueryable<QMessage> CreateQQueryable(InternalDbContext db, MessageFilter filter)
        {
            var q = db.Messages
              .AsNoTracking()
              .AsExpandable();

            if (filter.OrganizationIdentifier.HasValue)
            {
                q = q.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);
            }

            if (filter.Modified != null && !filter.Modified.IsEmpty)
            {
                if (filter.Modified.Since.HasValue)
                    q = q.Where(x => x.LastChangeTime >= filter.Modified.Since.Value);

                if (filter.Modified.Before.HasValue)
                    q = q.Where(x => x.LastChangeTime < filter.Modified.Before.Value);
            }

            if (filter.SurveyFormIdentifier.HasValue)
            {
                q = q.Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier);
            }

            if (filter.SenderIdentifier.HasValue)
            {
                q = q.Where(x => x.SenderIdentifier == filter.SenderIdentifier);
            }

            if (filter.Type.HasValue())
            {
                q = q.Where(x => x.MessageType == filter.Type);
            }

            return q;
        }

        private IQueryable<VMailout> CreateQueryable(InternalDbContext db, MailoutFilter filter)
        {
            var q = db.XMailouts
                .AsNoTracking()
                .AsExpandable();

            if (filter.PostOffice.IsNotEmpty())
                q = q.Where(x => x.SenderType == filter.PostOffice);

            if (filter.OrganizationIdentifier.HasValue)
            {
                q = q.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);
            }

            if (filter.MailoutIdentifier.HasValue && filter.MailoutIdentifier != Guid.Empty)
            {
                q = q.Where(x => x.MailoutIdentifier == filter.MailoutIdentifier);
            }

            if (filter.MessageIdentifier.HasValue && filter.MessageIdentifier != Guid.Empty)
            {
                q = q.Where(x => x.MessageIdentifier == filter.MessageIdentifier);
            }

            if (filter.EventIdentifier.HasValue && filter.EventIdentifier != Guid.Empty)
            {
                q = q.Where(x => x.EventIdentifier == filter.EventIdentifier);
            }

            if (filter.Sender.IsNotEmpty())
                q = q.Where(x => x.SenderEmail.Contains(filter.Sender) || x.SenderName.Contains(filter.Sender));

            if (filter.Subject.IsNotEmpty())
                q = q.Where(x => x.ContentSubject.Contains(filter.Subject));

            if (filter.Status.IsNotEmpty())
            {
                q = q.Where(x => x.MailoutStatus == filter.Status);
            }

            if (filter.MessageName.IsNotEmpty())
            {
                q = q.Where(x => x.MessageName == filter.MessageName);
            }

            if (filter.MessageType.IsNotEmpty())
            {
                q = q.Where(x => x.MessageType == filter.MessageType);
            }

            if (filter.MinDeliveryCount.HasValue)
            {
                q = q.Where(x => x.DeliveryCount >= filter.MinDeliveryCount);
            }

            if (filter.MaxDeliveryCount.HasValue)
            {
                q = q.Where(x => x.DeliveryCount <= filter.MaxDeliveryCount);
            }

            if (filter.Scheduled != null)
            {
                if (filter.Scheduled.Since.HasValue)
                {
                    q = q.Where(x => x.MailoutScheduled >= filter.Scheduled.Since.Value);
                }

                if (filter.Scheduled.Before.HasValue)
                {
                    q = q.Where(x => x.MailoutScheduled < filter.Scheduled.Before.Value);
                }
            }

            if (filter.Completed != null)
            {
                if (filter.Completed.Since.HasValue)
                {
                    q = q.Where(x => x.MailoutCompleted >= filter.Completed.Since.Value);
                }

                if (filter.Completed.Before.HasValue)
                {
                    q = q.Where(x => x.MailoutCompleted < filter.Completed.Before.Value);
                }
            }

            if (filter.IsCancelled.HasValue)
            {
                if (filter.IsCancelled.Value)
                {
                    q = q.Where(x => x.MailoutCancelled.HasValue);
                }
                else
                {
                    q = q.Where(x => !x.MailoutCancelled.HasValue);
                }
            }

            if (filter.IsCompleted.HasValue)
            {
                if (filter.IsCompleted.Value)
                {
                    q = q.Where(x => x.MailoutCompleted.HasValue);
                }
                else
                {
                    q = q.Where(x => !x.MailoutCompleted.HasValue);
                }
            }

            if (filter.IsLocked.HasValue)
            {
                if (filter.IsLocked.Value)
                {
                    q = q.Where(x => x.MailoutStatus == "Locked");
                }
                else
                {
                    q = q.Where(x => x.MailoutStatus != "Locked");
                }
            }

            if (filter.IsStarted.HasValue)
            {
                if (filter.IsStarted.Value)
                {
                    q = q.Where(x => x.MailoutStarted.HasValue);
                }
                else
                {
                    q = q.Where(x => !x.MailoutStarted.HasValue);
                }
            }

            if (filter.IsEmpty.HasValue)
            {
                if (filter.IsEmpty.Value)
                {
                    q = q.Where(x => x.DeliveryCount == 0);
                }
                else
                {
                    q = q.Where(x => x.DeliveryCount != 0);
                }
            }

            if (filter.Recipient.IsNotEmpty())
            {
                q = q.Where(x =>
                    db.Recipients.Any(y =>
                        y.MailoutIdentifier == x.MailoutIdentifier &&
                        (y.UserEmail.Contains(filter.Recipient) || y.PersonName.Contains(filter.Recipient))
                    )
                );
            }

            return q;
        }

        private IQueryable<QMailout> CreateQueryable2(InternalDbContext db, MailoutFilter filter)
        {
            var q = db.Mailouts
                .AsNoTracking()
                .AsExpandable();

            if (filter.PostOffice.IsNotEmpty())
                q = q.Where(x => x.SenderType == filter.PostOffice);

            if (filter.OrganizationIdentifier.HasValue)
                q = q.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.MailoutIdentifier.HasValue && filter.MailoutIdentifier != Guid.Empty)
                q = q.Where(x => x.MailoutIdentifier == filter.MailoutIdentifier);

            if (filter.MessageIdentifier.HasValue && filter.MessageIdentifier != Guid.Empty)
                q = q.Where(x => x.MessageIdentifier == filter.MessageIdentifier);

            if (filter.EventIdentifier.HasValue && filter.EventIdentifier != Guid.Empty)
                q = q.Where(x => x.EventIdentifier == filter.EventIdentifier);

            if (filter.Sender.IsNotEmpty())
                q = q.Where(x => db.TSenders.Any(y => y.SenderIdentifier == x.SenderIdentifier && (y.SenderEmail.Contains(filter.Sender) || y.SenderName.Contains(filter.Sender))));

            if (filter.Subject.IsNotEmpty())
                q = q.Where(x => x.ContentSubject.Contains(filter.Subject));

            if (filter.Status.IsNotEmpty())
                q = q.Where(x => x.MailoutStatus == filter.Status);

            if (filter.MessageType.IsNotEmpty())
                q = q.Where(x => x.MessageType == filter.MessageType);

            if (filter.MessageName.IsNotEmpty())
                q = q.Where(x => x.MessageName == filter.MessageName);

            if (filter.MinDeliveryCount.HasValue)
                q = q.Where(x => db.Recipients.Count(y => y.MailoutIdentifier == x.MailoutIdentifier) >= filter.MaxDeliveryCount);

            if (filter.MaxDeliveryCount.HasValue)
                q = q.Where(x => db.Recipients.Count(y => y.MailoutIdentifier == x.MailoutIdentifier) <= filter.MaxDeliveryCount);

            if (filter.Scheduled != null)
            {
                if (filter.Scheduled.Since.HasValue)
                    q = q.Where(x => x.MailoutScheduled >= filter.Scheduled.Since.Value);

                if (filter.Scheduled.Before.HasValue)
                    q = q.Where(x => x.MailoutScheduled < filter.Scheduled.Before.Value);
            }

            if (filter.Completed != null)
            {
                if (filter.Completed.Since.HasValue)
                    q = q.Where(x => x.MailoutCompleted >= filter.Completed.Since.Value);

                if (filter.Completed.Before.HasValue)
                    q = q.Where(x => x.MailoutCompleted < filter.Completed.Before.Value);
            }

            if (filter.IsCancelled.HasValue)
            {
                if (filter.IsCancelled.Value)
                    q = q.Where(x => x.MailoutCancelled.HasValue);
                else
                    q = q.Where(x => !x.MailoutCancelled.HasValue);
            }

            if (filter.IsCompleted.HasValue)
            {
                if (filter.IsCompleted.Value)
                    q = q.Where(x => x.MailoutCompleted.HasValue);
                else
                    q = q.Where(x => !x.MailoutCompleted.HasValue);
            }

            if (filter.IsLocked.HasValue)
            {
                if (filter.IsLocked.Value)
                    q = q.Where(x => x.MailoutStatus == "Locked");
                else
                    q = q.Where(x => x.MailoutStatus != "Locked");
            }

            if (filter.IsStarted.HasValue)
            {
                if (filter.IsStarted.Value)
                    q = q.Where(x => x.MailoutStarted.HasValue);
                else
                    q = q.Where(x => !x.MailoutStarted.HasValue);
            }

            if (filter.IsEmpty.HasValue)
            {
                if (filter.IsEmpty.Value)
                    q = q.Where(x => !db.Recipients.Any(y => y.MailoutIdentifier == x.MailoutIdentifier));
                else
                    q = q.Where(x => db.Recipients.Any(y => y.MailoutIdentifier == x.MailoutIdentifier));
            }

            if (filter.Recipient.IsNotEmpty())
            {
                q = q.Where(x =>
                    db.Recipients.Any(y =>
                        y.MailoutIdentifier == x.MailoutIdentifier &&
                        (y.UserEmail.Contains(filter.Recipient) || y.PersonName.Contains(filter.Recipient))
                    )
                );
            }

            return q;
        }

        private IQueryable<QRecipient> CreateQueryable(InternalDbContext db, DeliveryFilter filter)
        {
            var q = db.Recipients
                .Include(x => x.CarbonCopies)
                .AsNoTracking()
                .AsExpandable();

            if (filter.OrganizationIdentifier.HasValue)
                q = q.Where(x => x.Mailout.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.MessageIdentifier.HasValue)
                q = q.Where(x => x.Mailout.MessageIdentifier == filter.MessageIdentifier);

            if (filter.MailoutIdentifier.HasValue)
                q = q.Where(x => x.MailoutIdentifier == filter.MailoutIdentifier);

            if (filter.RecipientAddress.IsNotEmpty())
                q = q.Where(x => x.UserEmail == filter.RecipientAddress);

            if (filter.SurveyFormIdentifier.HasValue)
                q = q.Where(x => x.Mailout.SurveyIdentifier == filter.SurveyFormIdentifier);

            if (filter.Status.IsNotEmpty())
            {
                if (filter.Status == "Completed" || filter.Status == "Succeeded")
                    q = q.Where(x => x.DeliveryStatus == "Completed" || x.DeliveryStatus == "Succeeded" || x.Mailout.MailoutStatus == "Completed" || x.Mailout.MailoutStatus == "Succeeded");
                else
                    q = q.Where(x => x.DeliveryStatus == filter.Status || x.Mailout.MailoutStatus == filter.Status);
            }

            if (filter.Keyword.IsNotEmpty())
                q = q.Where(x => x.PersonName.Contains(filter.Keyword) || x.DeliveryError.Contains(filter.Keyword));

            return q;
        }

        public Guid[] GetOrphanMessages()
        {
            const string sql = @"
SELECT
    MessageIdentifier
FROM
    messages.QMessage
WHERE
    MessageName IN (
                       'AttemptCompleted', 'AttemptStarted', 'CapeMemberRegistrationSubmitted'
                     , 'IecbcContactLoginEnabled', 'SurveyResponseCompleted', 'SurveyResponseStarted', 'TenantChanged'
                     , 'WebinarScheduled'
                   )
    OR OrganizationIdentifier IN
           (
               SELECT
                   OrganizationIdentifier
               FROM
                   accounts.QOrganization
               WHERE
                   OrganizationCode = 'cape' OR AccountStatus = 'Closed'
           )
    OR MessageIdentifier NOT IN (SELECT AggregateIdentifier FROM logs.[Aggregate])
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Guid>(sql).ToArray();
        }

        public string GetCarbonCopyEmails(ICollection<QCarbonCopy> carbonCopies)
        {
            if (carbonCopies == null || carbonCopies.Count == 0)
                return null;

            using (var db = new InternalDbContext())
            {
                var identifiers = carbonCopies.Select(x => x.UserIdentifier).ToArray();

                if (identifiers.Length == 0)
                    return null;

                var emails = db.QUsers
                    .Where(x => identifiers.Contains(x.UserIdentifier))
                    .Select(x => x.Email)
                    .ToArray();

                if (emails.Length == 0)
                    return null;

                return string.Join(", ", emails);
            }
        }
    }
}