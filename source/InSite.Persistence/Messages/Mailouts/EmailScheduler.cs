using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;
using InSite.Persistence.Integration.DirectAccess;

using Serilog;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class EmailScheduler
    {
        #region Fields

        private readonly EnvironmentName _environment;
        private readonly string[] _applicationDomains;
        private readonly string[] _applicationTesters;
        private readonly IMessageSearch _messageSearch;
        private readonly ILogger _logger;
        private readonly DirectAccessMailer _mailServer;

        public static object StaticLock = new object();

        #endregion

        public EmailScheduler(
            EnvironmentName environment,
            string[] applicationDomains,
            string[] applicationTesters,
            ILogger logger,
            IMessageSearch messageSearch,
            DirectAccessMailer mailServer
            )
        {
            _environment = environment;
            _applicationDomains = applicationDomains;
            _applicationTesters = applicationTesters;
            _logger = logger;
            _messageSearch = messageSearch;
            _mailServer = mailServer;
        }

        public void Execute(int deliveryLimit)
        {
            // Ensure no new mailout job can be created before another mailout job has completed sending.

            lock (StaticLock)
            {
                var jobs = CreateJobs(_mailServer.SenderType, deliveryLimit);

                if (jobs.Count == 0)
                    return;

                _mailServer.Send(jobs);
            }
        }

        private List<IDeliveryJob> CreateJobs(SenderType type, int deliveryLimit)
        {
            var result = new List<IDeliveryJob>();

            var now = DateTimeOffset.UtcNow;
            var twoWeeksAgo = now.AddDays(-14);

            var filter = new MailoutFilter
            {
                PostOffice = type.ToString(),
                Scheduled = new DateTimeOffsetRange(twoWeeksAgo, now),
                IsStarted = false,
                IsCancelled = false,
                IsCompleted = false,
                IsLocked = false
            };

            var queue = _messageSearch.GetMailouts(filter)
                .Where(x => x.MessageIdentifier != null && x.MessageIdentifier != Guid.Empty)
                .OrderByDescending(x => x.MailoutScheduled)
                .ToList();

            if (deliveryLimit > 0)
                queue = queue.Take(deliveryLimit).ToList();

            if (queue.Count > 0)
                _logger.Information($"Scheduled Mailouts ({type}) = {queue.Count}");

            var mailouts = new List<VMailout>();
            foreach (var item in queue)
                mailouts.Add(item);

            if (mailouts.Count == 0)
                return result;

            foreach (var mailout in mailouts)
            {
                var deliveries = _messageSearch.GetDeliveries(mailout.MailoutIdentifier);
                var jobs = DeliveryAdapter.CreateEmailJobs(mailout, deliveries);

                ValidateRecipients(jobs);

                foreach (var job in jobs)
                    if (job.Email.Recipients.Any())
                        result.Add(job);
            }

            var recipientCount = result.Select(x => x.Email).Sum(x => x.Recipients.Count);

            _logger.Information($"  Validated Recipients: {recipientCount}");

            return result;
        }

        private void ValidateRecipients(IEnumerable<IDeliveryJob> jobs)
        {
            foreach (var job in jobs)
            {
                var excludes = new List<Guid>();

                foreach (var to in job.Email.Recipients)
                {
                    // The email address for the recipient MUST be enabled.

                    if (to.Address.IsEmpty() || !EmailAddress.IsValidAddress(to.Address))
                        excludes.Add(to.Identifier.Value);

                    // If we are not running in Production then every recipient must be a tester OR the recipient's
                    // mailbox domain must be an application domain name.

                    else if (_environment != EnvironmentName.Production)
                    {
                        if (StringHelper.EqualsAny(to.Address, _applicationTesters))
                            continue;

                        var email = new EmailAddress(to.Address);
                        if (StringHelper.EqualsAny(email.Domain, _applicationDomains))
                            continue;

                        excludes.Add(to.Identifier.Value);
                    }
                }

                // Exclude recipients who should not receive email from the system.

                foreach (var exclude in excludes)
                    job.Email.Recipients.Remove(exclude);
            }
        }
    }
}