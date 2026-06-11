using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public static class MessageAdapter
    {
        #region Classes

        private class InternalRecipientData
        {
            public int DeliveryCount { get; set; }
            public bool HasReponse { get; set; }
            public DateTimeOffset? StatusDate { get; set; }
        }

        public class DeliveryInfo
        {
            public Guid OrganizationIdentifier { get; private set; }
            public DateTimeOffset Scheduled { get; private set; }
            public DateTimeOffset? Completed { get; private set; }

            public IReadOnlyList<string> Recipients { get; private set; }

            public DeliveryInfo(QMailout mailout, IEnumerable<string> recipients)
            {
                OrganizationIdentifier = mailout.OrganizationIdentifier;
                Scheduled = mailout.MailoutScheduled;
                Completed = mailout.MailoutCompleted;

                Recipients = new List<string>(recipients);
            }
        }

        public class SingleInvitationInfo
        {
            public string Email { get; private set; }
            public DateTimeOffset StatusDate { get; private set; }

            public SingleInvitationInfo(string email, DateTimeOffset statusDate)
            {
                Email = email;
                StatusDate = statusDate;
            }
        }

        public class InvitationInfo
        {
            public string Email { get; private set; }
            public DateTimeOffset StatusDate { get; private set; }
            public int DeliveryCount { get; private set; }

            public InvitationInfo(string email, DateTimeOffset statusDate, int deliveryCount)
            {
                Email = email;
                StatusDate = statusDate;
                DeliveryCount = deliveryCount;
            }
        }

        public class StatisticInfo
        {
            public List<DeliveryInfo> DeliveryScheduled { get; private set; }
            public List<DeliveryInfo> DeliveryDelivered { get; private set; }

            public List<SingleInvitationInfo> InvitationDelivered { get; private set; }
            public List<SingleInvitationInfo> InvitationNotDelivered { get; private set; }
            public List<InvitationInfo> InvitationDeliveredMoreThanOnce { get; private set; }
            public List<SingleInvitationInfo> InvitatioResponded { get; private set; }

            public StatisticInfo()
            {
                DeliveryScheduled = new List<DeliveryInfo>();
                DeliveryDelivered = new List<DeliveryInfo>();

                InvitationDelivered = new List<SingleInvitationInfo>();
                InvitationNotDelivered = new List<SingleInvitationInfo>();
                InvitationDeliveredMoreThanOnce = new List<InvitationInfo>();
                InvitatioResponded = new List<SingleInvitationInfo>();
            }
        }

        #endregion

        public static StatisticInfo GetInvitationStatistic(Guid surveyFormIdentifier)
        {
            var result = new StatisticInfo();

            List<Guid> messageThubmbprints;

            {
                using (var db = new InternalDbContext(false))
                {
                    messageThubmbprints = db.Messages
                        .Where(x => x.SurveyFormIdentifier == surveyFormIdentifier)
                        .Select(x => x.MessageIdentifier)
                        .ToList();
                }
            }

            if (messageThubmbprints.Count > 0)
            {
                var recipients = new Dictionary<string, InternalRecipientData>();

                using (var db = new InternalDbContext(false))
                {
                    db.Mailouts
                        .Where(
                            x => x.MessageIdentifier.HasValue
                                && messageThubmbprints.Contains(x.MessageIdentifier.Value)
                                && !x.MailoutCompleted.HasValue
                                && !x.MailoutCancelled.HasValue)
                        .OrderByDescending(x => x.MailoutScheduled)
                        .Select(x => new { Delivery = x, x.Recipients })
                        .ToList()
                        .ForEach(x =>
                        {
                            var deliveryInfo = new DeliveryInfo(x.Delivery, x.Recipients.Select(y => y.UserEmail));

                            result.DeliveryScheduled.Add(deliveryInfo);
                        });

                    db.Mailouts
                        .Include(x => x.Recipients)
                        .Where(
                            x => x.MessageIdentifier.HasValue
                                && messageThubmbprints.Contains(x.MessageIdentifier.Value)
                                && x.MailoutCompleted.HasValue
                                && !x.MailoutCancelled.HasValue)
                        .ToList()
                        .ForEach(x =>
                        {
                            var deliveryEmails = new HashSet<string>();

                            foreach (var status in x.Recipients)
                            {
                                var key = status.UserEmail.Trim().ToLower();

                                if (!recipients.ContainsKey(key))
                                    recipients.Add(key, new InternalRecipientData());

                                var recipient = recipients[key];

                                if (status.DeliveryError == null)
                                {
                                    recipient.DeliveryCount++;
                                    recipient.StatusDate = status.DeliveryCompleted;

                                    if (!deliveryEmails.Contains(key))
                                        deliveryEmails.Add(key);
                                }
                                else if (recipient.DeliveryCount == 0)
                                    recipient.StatusDate = status.DeliveryCompleted;
                            }

                            var deliveryInfo = new DeliveryInfo(x, deliveryEmails.ToList());

                            result.DeliveryDelivered.Add(deliveryInfo);
                        });
                }

                foreach (var kv in recipients)
                {
                    DateTimeOffset status = kv.Value.StatusDate ?? DateTimeOffset.MinValue;

                    var deliveredCount = kv.Value.DeliveryCount;
                    if (deliveredCount > 0)
                    {
                        result.InvitationDelivered.Add(new SingleInvitationInfo(kv.Key, status));

                        if (deliveredCount > 1)
                            result.InvitationDeliveredMoreThanOnce.Add(new InvitationInfo(kv.Key, status, deliveredCount));
                    }
                    else
                    {
                        result.InvitationNotDelivered.Add(new SingleInvitationInfo(kv.Key, status));
                    }
                }

                foreach (var removal in result.InvitationNotDelivered)
                    recipients.Remove(removal.Email);

                if (recipients.Count > 0)
                {
                    using (var irisDb = new InternalDbContext())
                    {
                        irisDb.QResponseSessions
                            .Where(x => x.SurveyForm.SurveyFormIdentifier == surveyFormIdentifier && (x.ResponseSessionCompleted.HasValue || x.ResponseSessionStarted.HasValue))
                            .Select(x => new
                            {
                                Email = x.Respondent.UserEmail,
                                Date = x.ResponseSessionCompleted ?? x.ResponseSessionStarted
                            })
                            .OrderBy(x => x.Email).ThenByDescending(x => x.Date)
                            .ToList()
                            .ForEach(x =>
                            {
                                if (string.IsNullOrEmpty(x.Email) || !x.Date.HasValue)
                                    return;

                                var key = x.Email.ToLower();
                                if (!recipients.ContainsKey(key))
                                    return;

                                var recipient = recipients[key];
                                if (recipient.HasReponse)
                                    return;

                                result.InvitatioResponded.Add(new SingleInvitationInfo(x.Email, x.Date.Value));

                                recipient.HasReponse = true;
                            });
                    }
                }
            }

            return result;
        }
    }
}