using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Persistence
{
    public class MessageStore : IMessageStore
    {
        private readonly string _domain;

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public MessageStore(string domain)
        {
            _domain = domain;
        }

        public QLink InsertLink(Guid id)
        {
            using (var db = CreateContext())
            {
                var link = db.Links
                    .AsNoTracking()
                    .FirstOrDefault(x => x.LinkIdentifier == id);

                if (link == null)
                {
                    var href = $"https://www.{_domain}/library/images/blank.jpg";
                    var l = new QLink
                    {
                        LinkIdentifier = id,
                        LinkText = "Open/Read Message",
                        LinkUrl = href,
                        LinkUrlHash = HashHelper.MD5(href),
                        MessageIdentifier = id
                    };
                    db.Links.Add(l);
                    db.SaveChanges();
                }

                return link;
            }
        }

        #region Methods (messages)

        public void Delete(Guid aggregate)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE [messages].QClick           WHERE LinkIdentifier IN (SELECT LinkIdentifier FROM [messages].QLink WHERE [MessageIdentifier] = @Aggregate);
DELETE [messages].QLink            WHERE [MessageIdentifier] = @Aggregate;
DELETE communications.QRecipient   WHERE [MessageIdentifier] = @Aggregate;
DELETE [messages].QFollower        WHERE [MessageIdentifier] = @Aggregate;
DELETE communications.QMailout     WHERE [MessageIdentifier] = @Aggregate;
DELETE [messages].QMessage         WHERE [MessageIdentifier] = @Aggregate;
DELETE [messages].QSubscriberGroup WHERE [MessageIdentifier] = @Aggregate;
DELETE [messages].QSubscriberUser  WHERE [MessageIdentifier] = @Aggregate;
";
                db.Database.CommandTimeout = 5 * 60; // 5 minutes
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", aggregate));
            }
        }

        public void InsertMessage(MessageCreated e)
        {
            using (var db = CreateContext())
            {
                var message = new QMessage
                {
                    MessageIdentifier = e.AggregateIdentifier,

                    MessageType = e.Type,
                    MessageName = e.Name,
                    MessageTitle = e.Title.Default ?? "None",

                    ContentText = e.ContentText.Default,

                    SurveyFormIdentifier = e.SurveyFormIdentifier,

                    OrganizationIdentifier = e.Tenant
                };

                if (e.Sender != null)
                    message.SenderIdentifier = e.Sender;
                else
                    message.SenderIdentifier = Guid.Empty;

                SetLastChange(message, e);

                db.Messages.Add(message);

                if (e.Links != null)
                    foreach (var item in e.Links)
                    {
                        if (string.IsNullOrEmpty(item.Href))
                        {
                            continue;
                        }

                        var link = db.Links.FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier && x.LinkUrl == item.Href);

                        if (link == null)
                        {
                            link = new QLink
                            {
                                LinkIdentifier = UniqueIdentifier.Create(),
                                LinkText = item.Text ?? item.Href,
                                LinkUrl = item.Href,
                                LinkUrlHash = HashHelper.MD5(item.Href),
                                MessageIdentifier = e.AggregateIdentifier
                            };

                            if (string.IsNullOrEmpty(link.LinkText))
                            {
                                link.LinkText = link.LinkUrl;
                            }

                            db.Links.Add(link);
                        }
                    }

                db.SaveChanges();
            }
        }

        public void UpdateMessage(Classified e)
        {
            UpdateMessage(e, message => { });

            using (var db = CreateContext())
            {
                var message = db.Messages.FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier);

                if (message == null)
                    return;

                db.SaveChanges();
            }
        }

        public void UpdateMessage(ContentChanged e)
        {
            UpdateMessage(e, message =>
            {
                message.ContentText = e.ContentText.Default;
            });

            using (var db = CreateContext())
            {
                foreach (var item in e.Links)
                {
                    if (!db.Links.Any(x => x.MessageIdentifier == e.AggregateIdentifier && x.LinkUrl == item.Href))
                        db.Links.Add(new QLink
                        {
                            LinkIdentifier = UniqueIdentifier.Create(),
                            LinkText = item.Text,
                            LinkUrl = item.Href,
                            LinkUrlHash = HashHelper.MD5(item.Href),
                            MessageIdentifier = e.AggregateIdentifier
                        });
                }

                db.SaveChanges();
            }
        }

        public void UpdateMessage(LinkCounterReset e)
        {
            UpdateMessage(e, message => { });

            using (var db = CreateContext())
            {
                var link = db.Links.FirstOrDefault(x => x.LinkIdentifier == e.LinkIdentifier);
                if (link != null)
                {
                    link.ClickCount = 0;
                    link.UserCount = 0;

                    var clicks = db.Clickthroughs.Where(x => x.LinkIdentifier == e.LinkIdentifier);
                    db.Clickthroughs.RemoveRange(clicks);

                    var message = db.Messages.Single(x => link.MessageIdentifier == x.MessageIdentifier);
                    if ((message.ContentText != null && !message.ContentText.Contains(link.LinkUrl))
                      || (message.ContentHtml != null && !message.ContentHtml.Contains(link.LinkUrl)))
                        db.Links.Remove(link);

                    db.SaveChanges();
                }
            }
        }

        public void UpdateMessage(MessageArchived e)
        {
            UpdateMessage(e, message => { });

            using (var db = CreateContext())
            {
                var email = db.Messages.FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier);

                if (email == null)
                    return;

                db.Messages.Remove(email);
                db.SaveChanges();
            }
        }

        public void UpdateMessage(MessageDisabled e)
        {
            UpdateMessage(e, message =>
            {
                message.IsDisabled = true;
            });
        }

        public void UpdateMessage(MessageEnabled e)
        {
            UpdateMessage(e, message =>
            {
                message.IsDisabled = false;
            });
        }

        public void UpdateMessage(AutoBccSubscribersDisabled e)
        {
            UpdateMessage(e, message =>
            {
                message.AutoBccSubscribers = false;
            });
        }

        public void UpdateMessage(AutoBccSubscribersEnabled e)
        {
            UpdateMessage(e, message =>
            {
                message.AutoBccSubscribers = true;
            });
        }

        public void UpdateMessage(MessageRenamed e)
        {
            UpdateMessage(e, message =>
            {
                message.MessageName = e.Name;
            });
        }

        public void UpdateMessage(MessageRetitled e)
        {
            UpdateMessage(e, message =>
            {
                message.MessageTitle = e.Title.Default;
            });
        }

        public void UpdateMessage(SenderChanged e)
        {
            UpdateMessage(e, message =>
            {
                message.SenderIdentifier = e.Sender;
            });
        }

        public void UpdateMessage(SurveyFormAssigned e)
        {
            UpdateMessage(e, message =>
            {
                message.SurveyFormIdentifier = e.SurveyFormIdentifier;
            });
        }

        private void UpdateMessage(IChange e, Action<QMessage> change)
        {
            using (var db = CreateContext())
            {
                var message = db.Messages
                    .FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier);

                if (message == null)
                    return;

                change(message);

                SetLastChange(message, e);

                db.SaveChanges();
            }
        }

        private void SetLastChange(QMessage entity, IChange change)
        {
            entity.LastChangeTime = change.ChangeTime;
            entity.LastChangeType = change.GetType().Name;
            entity.LastChangeUser = change.OriginUser;
        }

        #endregion

        #region Methods (mailouts)

        public void UpdateMessage(MailoutAborted e)
        {
            UpdateMailout(e.Mailout, mailout =>
            {
                mailout.MailoutCompleted = e.ChangeTime;
                mailout.MailoutStatus = "Aborted";
                mailout.MailoutError = e.Reason;
            });
        }

        public void UpdateMessage(MailoutCallbackHandled e)
        {
            if (e.Recipient.IsEmpty())
                return;

            var messageState = (MessageState)e.AggregateState;
            var mailoutState = messageState.FindMailout(e.Mailout);
            var recipientState = mailoutState.GetRecipient(e.Recipient);

            UpdateMailout(e.Mailout, mailout =>
            {
                mailout.MailoutStatus = mailoutState.Status;
                mailout.MailoutStatusDescription = mailoutState.StatusDescription.MaxLength(128);

                if (!mailout.MailoutCompleted.HasValue && mailoutState.IsCompleted())
                    mailout.MailoutCompleted = e.ChangeTime;

                var allowHandle = e.Status == MailoutCallbackStatus.Accepted
                    || e.Status == MailoutCallbackStatus.Delivered
                    || e.Status == MailoutCallbackStatus.Failed
                    || e.Status == MailoutCallbackStatus.Rejected;
                if (!allowHandle)
                    return;

                var recipient = mailout.Recipients.FirstOrDefault(x => x.UserEmail == e.Recipient);
                if (recipient == null)
                    return;

                if (recipient.DeliveryCallbackTimestamp.HasValue && recipient.DeliveryCallbackTimestamp.Value > e.Timestamp)
                    return;

                recipient.DeliveryStatus = recipientState.CallbackStatus;
                recipient.DeliveryError = null;

                var data = e.Data;
                if (data.IsEmpty())
                    return;

                if (e.Status == MailoutCallbackStatus.Failed)
                    recipient.DeliveryError = $"[{data.GetOrDefault("code")}] {data.GetOrDefault("message")}";
            });
        }

        public void UpdateMessage(MailoutCancelled e)
        {
            UpdateMailout(e.MailoutIdentifier, mailout => { });

            using (var db = CreateContext())
            {
                var mailouts = db.Mailouts.Where(x => x.MailoutIdentifier == e.MailoutIdentifier);
                db.Mailouts.RemoveRange(mailouts);
                var deliveries = db.Recipients.Where(x => x.MailoutIdentifier == e.MailoutIdentifier);
                db.Recipients.RemoveRange(deliveries);
                db.SaveChanges();
            }
        }

        public void UpdateMessage(MailoutCompleted e)
        {
            UpdateMailout(e.MailoutIdentifier, mailout =>
            {
                mailout.MailoutCompleted = e.ChangeTime;
                mailout.MailoutStatus = "Completed";
            });
        }

        private static readonly int? MaxContentSubject = InternalDbContext.GetMaxLength<QMailout>(x => x.ContentSubject);
        private static readonly int? MaxMailoutStatusCode = InternalDbContext.GetMaxLength<QMailout>(x => x.MailoutStatusCode);
        private static readonly int? MaxSenderStatus = InternalDbContext.GetMaxLength<QMailout>(x => x.SenderStatus);

        public void UpdateMessage(MailoutDrafted e)
        {
            using (var db = CreateContext())
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var message = db.Messages.FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier)
                    ?? throw new ArgumentException($"Message {e.AggregateIdentifier} does not exist");

                var mailout = new QMailout
                {
                    MessageIdentifier = e.AggregateIdentifier,
                    MessageType = message.MessageType,
                    MessageName = message.MessageName,
                    SurveyIdentifier = message.SurveyFormIdentifier,
                    OrganizationIdentifier = message.OrganizationIdentifier,

                    SenderType = e.SenderType,
                    SenderIdentifier = e.SenderId,

                    MailoutIdentifier = e.MailoutId,
                    MailoutStatus = "Drafted",
                    MailoutScheduled = e.ScheduledOn,

                    EventIdentifier = e.EventId,

                    ContentSubject = MaxContentSubject.HasValue ? e.Subject.Default.MaxLength(MaxContentSubject.Value) : e.Subject.Default,
                    ContentBodyText = e.BodyText?.Default,
                    ContentBodyHtml = e.BodyHtml?.Default,
                    ContentAttachments = e.Attachments.IsNotEmpty() ? JsonConvert.SerializeObject(e.Attachments) : null,
                };

                var to = new Dictionary<Guid, string>();
                var cc = new Dictionary<Guid, string>();

                if (e.To.IsNotEmpty())
                {
                    foreach (var item in e.To)
                        if (!to.ContainsKey(item.Key))
                            to.Add(item.Key, item.Value);
                }

                if (e.Cc.IsNotEmpty())
                {
                    foreach (var item in e.Cc)
                        if (!cc.ContainsKey(item.Key))
                            cc.Add(item.Key, item.Value);
                }

                if (e.Bcc.IsNotEmpty())
                {
                    foreach (var item in e.Bcc)
                        if (!cc.ContainsKey(item.Key))
                            cc.Add(item.Key, item.Value);
                }

                var users = db.Persons.Where(x => x.OrganizationIdentifier == message.OrganizationIdentifier)
                    .Select(x => new { x.UserIdentifier, x.User.Email, x.FullName, x.Language, x.PersonCode })
                    .Distinct()
                    .ToDictionary(x => x.UserIdentifier, x => x);

                foreach (var itemTo in to)
                {
                    if (!users.TryGetValue(itemTo.Key, out var user))
                        continue;

                    var recipient = new QRecipient
                    {
                        RecipientIdentifier = UniqueIdentifier.Create(),
                        OrganizationIdentifier = mailout.OrganizationIdentifier,
                        MailoutIdentifier = mailout.MailoutIdentifier,
                        DeliveryStatus = "Drafted",

                        UserEmail = itemTo.Value,
                        UserIdentifier = itemTo.Key,
                        PersonName = user.FullName,
                        PersonCode = user.PersonCode,
                        PersonLanguage = user.Language,
                    };

                    foreach (var itemCc in cc)
                    {
                        if (users.ContainsKey(itemCc.Key))
                            recipient.CarbonCopies.Add(new QCarbonCopy
                            {
                                CarbonCopyIdentifier = UniqueIdentifier.Create(),
                                OrganizationIdentifier = mailout.OrganizationIdentifier,
                                RecipientIdentifier = recipient.RecipientIdentifier,
                                UserIdentifier = itemCc.Key
                            });
                    }

                    db.Recipients.Add(recipient);
                }

                db.Mailouts.Add(mailout);

                db.SaveChanges();
            }
        }

        public void UpdateMessage(MailoutQueued e)
        {
            if (e.Recipient.IsEmpty())
                return;

            var messageState = (MessageState)e.AggregateState;
            var mailoutState = messageState.FindMailout(e.MailoutId);
            var recipientState = mailoutState.GetRecipient(e.Recipient);

            UpdateMailout(e.MailoutId, mailout =>
            {
                mailout.MailoutStatus = mailoutState.Status;
                mailout.MailoutStatusDescription = mailoutState.StatusDescription.MaxLength(128);

                var recipient = mailout.Recipients.FirstOrDefault(x => x.UserEmail == e.Recipient);
                if (recipient == null || recipient.DeliveryStatus != MailoutCallbackStatus.Drafted)
                    return;

                recipient.DeliveryStatus = recipientState.CallbackStatus;
                recipient.DeliveryError = null;
            });
        }

        public void UpdateMessage(MailoutRejected e)
        {
            if (e.Recipient.IsEmpty())
                return;

            var messageState = (MessageState)e.AggregateState;
            var mailoutState = messageState.FindMailout(e.MailoutId);

            UpdateMailout(e.MailoutId, mailout =>
            {
                mailout.MailoutStatus = mailoutState.Status;
                mailout.MailoutStatusDescription = mailoutState.StatusDescription.MaxLength(128);

                if (!mailout.MailoutCompleted.HasValue && mailoutState.IsCompleted())
                    mailout.MailoutCompleted = e.ChangeTime;

                if (e.Recipient != null)
                {
                    var recipient = mailout.Recipients.FirstOrDefault(x => x.UserEmail == e.Recipient);
                    UpdateRecipient(recipient);
                }
                else
                {
                    foreach (var recipient in mailout.Recipients)
                        UpdateRecipient(recipient);
                }
            });

            void UpdateRecipient(QRecipient r)
            {
                if (r == null || r.DeliveryStatus != MailoutCallbackStatus.Drafted && r.DeliveryStatus != MailoutCallbackStatus.Queued)
                    return;

                var recipientState = mailoutState.GetRecipient(r.UserEmail);
                if (recipientState == null)
                    return;

                r.DeliveryStatus = recipientState.CallbackStatus;
                r.DeliveryError = e.Description;
            }
        }

        public void UpdateMessage(MailoutScheduled2 e)
        {
            UpdateMailout(e.MailoutIdentifier, mailout => { });

            var sender = TSenderSearch.Select(e.SenderIdentifier);

            using (var db = CreateContext())
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var message = db.Messages.FirstOrDefault(x => x.MessageIdentifier == e.AggregateIdentifier);
                if (message == null)
                    return;

                var mailout = new QMailout
                {
                    EventIdentifier = e.EventIdentifier,

                    ContentBodyText = e.BodyText.Default,

                    MailoutIdentifier = e.MailoutIdentifier,
                    MailoutStatus = "Scheduled",
                    MailoutScheduled = e.At,

                    MessageIdentifier = e.AggregateIdentifier,
                    MessageType = message.MessageType,
                    MessageName = message.MessageName,
                    ContentSubject = e.Subject.Default,
                    ContentVariables = JsonConvert.SerializeObject(e.Variables),

                    SenderType = sender.SenderType,
                    SenderIdentifier = e.SenderIdentifier,

                    SurveyIdentifier = message.SurveyFormIdentifier,
                    OrganizationIdentifier = message.OrganizationIdentifier
                };

                var sentToEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var recipient in e.Recipients)
                {
                    var cc = new List<Guid>();
                    if (recipient.Cc != null)
                    {
                        if (recipient.Bcc != null)
                            cc = recipient.Cc.Union(recipient.Bcc).ToList();
                        else
                            cc = recipient.Cc.ToList();
                    }

                    var delivery = new QRecipient
                    {
                        RecipientIdentifier = UniqueIdentifier.Create(),
                        MailoutIdentifier = e.MailoutIdentifier,
                        UserIdentifier = recipient.Identifier.Value,
                        OrganizationIdentifier = message.OrganizationIdentifier,

                        UserEmail = recipient.Address,
                        PersonName = recipient.DisplayName,
                        PersonCode = recipient.Code,
                        PersonLanguage = recipient.Language,

                        RecipientVariables = JsonConvert.SerializeObject(recipient.Variables),
                        DeliveryStatus = "Scheduled"
                    };

                    delivery.CarbonCopies = cc
                        .Select(x => new QCarbonCopy
                        {
                            CarbonCopyIdentifier = UniqueIdentifier.Create(),
                            OrganizationIdentifier = message.OrganizationIdentifier,
                            UserIdentifier = x
                        })
                        .ToList();

                    db.Recipients.Add(delivery);

                    sentToEmails.Add(recipient.Address);
                }

                if (e.Variables != null && e.Variables.Any())
                    mailout.ContentVariables = JsonConvert.SerializeObject(e.Variables);

                if (e.Attachments != null && e.Attachments.Any())
                    mailout.ContentAttachments = JsonConvert.SerializeObject(e.Attachments);

                db.Mailouts.Add(mailout);

                db.SaveChanges();
            }
        }

        public void UpdateMessage(MailoutStarted e)
        {
            UpdateMailout(e.MailoutIdentifier, mailout =>
            {
                mailout.MailoutStarted = e.ChangeTime;
            });
        }

        private void UpdateMailout(Guid mailout, Action<QMailout> change)
        {
            using (var db = CreateContext())
            {
                var m = db.Mailouts
                    .FirstOrDefault(x => x.MailoutIdentifier == mailout);

                if (m == null)
                    return;

                change(m);

                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (deliveries)

        public void UpdateMessage(CarbonCopyCompleted2 e)
        {

        }

        public void UpdateMessage(CarbonCopyStarted2 e)
        {

        }

        public void UpdateMessage(DeliveryBounced e)
        {

        }

        public void UpdateMessage(DeliveryCompleted2 e)
        {
            Guid? organizationIdentifier = null;
            Guid? surveyFormIdentifier = null;

            using (var db = CreateContext())
            {
                var delivery = db.Recipients.FirstOrDefault(x => x.MailoutIdentifier == e.MailoutIdentifier && x.UserIdentifier == e.RecipientIdentifier);
                if (delivery == null)
                    return;

                delivery.DeliveryStatus = e.Error.IsEmpty() ? "Succeeded" : "Failed";
                delivery.DeliveryError = e.Error;
                delivery.DeliveryCompleted = e.ChangeTime;

                var mailout = db.Mailouts.FirstOrDefault(x => x.MailoutIdentifier == e.MailoutIdentifier);

                if (mailout != null)
                    organizationIdentifier = mailout.OrganizationIdentifier;

                db.SaveChanges();

                surveyFormIdentifier = delivery.Mailout.SurveyIdentifier;
            }
        }

        public void UpdateMessage(DeliveryStarted2 e)
        {
            using (var db = CreateContext())
            {
                var delivery = db.Recipients.FirstOrDefault(x => x.MailoutIdentifier == e.MailoutIdentifier && x.RecipientIdentifier == e.RecipientIdentifier);
                if (delivery == null)
                    return;

                delivery.DeliveryStatus = "Started";
                delivery.DeliveryStarted = e.ChangeTime;

                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (feedback)

        public void InsertClickthrough(Guid id, Guid user, string ip, string browser)
        {
            using (var db = CreateContext())
            {
                var link = db.Links.FirstOrDefault(x => x.LinkIdentifier == id);
                if (link == null)
                {
                    return;
                }

                link.ClickCount++;

                var exists = db.Clickthroughs.Any(x => x.LinkIdentifier == id && x.UserIdentifier == user);
                if (!exists)
                {
                    link.UserCount++;
                }

                var clickthrough = new QClick
                {
                    ClickIdentifier = UniqueIdentifier.Create(),
                    Clicked = DateTimeOffset.UtcNow,
                    LinkIdentifier = id,
                    UserIdentifier = user,
                    UserHostAddress = ip,
                    UserBrowser = browser
                };

                db.Clickthroughs.Add(clickthrough);

                db.SaveChanges();
            }
        }

        #endregion

        #region Contacts

        public void InsertSubscriberUser(Guid message, Guid user, DateTimeOffset time)
        {
            using (var db = CreateContext())
            {
                var exists = db.SubscriberUsers
                    .Any(x => x.MessageIdentifier == message && x.UserIdentifier == user);

                if (exists)
                    return;

                db.SubscriberUsers.Add(new QSubscriberUser
                {
                    MessageIdentifier = message,
                    UserIdentifier = user,
                    Subscribed = time
                });
                db.SaveChanges();
            }
        }

        public void InsertSubscriberGroup(Guid message, Guid group, DateTimeOffset time)
        {
            using (var db = CreateContext())
            {
                var exists = db.SubscriberGroups
                    .Any(x => x.MessageIdentifier == message && x.GroupIdentifier == group);

                if (exists)
                    return;

                db.SubscriberGroups.Add(new QSubscriberGroup
                {
                    MessageIdentifier = message,
                    GroupIdentifier = group,
                    Subscribed = time
                });
                db.SaveChanges();
            }
        }

        public void DeleteSubscriberUser(Guid message, Guid user)
        {
            using (var db = CreateContext())
            {
                var subscriber = db.SubscriberUsers
                    .FirstOrDefault(x => x.MessageIdentifier == message && x.UserIdentifier == user);

                if (subscriber != null)
                {
                    db.SubscriberUsers.Remove(subscriber);
                    db.SaveChanges();
                }
            }
        }

        public void DeleteSubscriberGroup(Guid message, Guid group)
        {
            using (var db = CreateContext())
            {
                var subscriber = db.SubscriberGroups
                    .FirstOrDefault(x => x.MessageIdentifier == message && x.GroupIdentifier == group);

                if (subscriber != null)
                {
                    db.SubscriberGroups.Remove(subscriber);
                    db.SaveChanges();
                }
            }
        }

        public void InsertFollower(Guid aggregate, Guid subscriber, Guid follower)
        {
            if (subscriber == follower)
                return;

            using (var db = CreateContext())
            {
                var exists = db.Followers.Any(
                    x => x.MessageIdentifier == aggregate
                      && x.SubscriberIdentifier == subscriber
                      && x.FollowerIdentifier == follower);

                if (exists)
                    return;

                db.Followers.Add(new QFollower
                {
                    JoinIdentifier = UniqueIdentifier.Create(),
                    MessageIdentifier = aggregate,
                    SubscriberIdentifier = subscriber,
                    FollowerIdentifier = follower
                });

                db.SaveChanges();
            }
        }

        public void DeleteFollower(Guid aggregate, Guid contact, Guid follower)
        {
            using (var db = CreateContext())
            {
                var attachment = db.Followers.FirstOrDefault(x => x.MessageIdentifier == aggregate && x.SubscriberIdentifier == contact && x.FollowerIdentifier == follower);
                if (attachment != null)
                {
                    db.Followers.Remove(attachment);
                    db.SaveChanges();
                }
            }
        }

        #endregion
    }
}