using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application;
using InSite.Application.Messages.Write;
using InSite.Domain.Messages;

using Serilog;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public class DirectAccessMailer
    {
        private class MailoutInfo
        {
            public Guid MessageIdentifier { get; set; }
            public Guid MailoutIdentifier { get; set; }
            public DateTimeOffset Scheduled { get; set; }
            public DateTimeOffset? Completed { get; set; }
            public DateTimeOffset? Cancelled { get; set; }

            public DeliveryInfo[] Deliveries { get; set; }
        }

        private class DeliveryInfo
        {
            public string Language { get; set; }
            public EmailDraft[] Emails { get; set; }
        }

        public SenderType SenderType => SenderType.DirectAccess;

        private readonly ILogger _logger;
        private readonly Action<ICommand> _send;
        private readonly IDirectAccessClient _da;
        private readonly IIdentityService _identityService;

        public DirectAccessMailer(ILogger logger, Action<ICommand> send, IDirectAccessClient da, IIdentityService identityService)
        {
            _logger = logger;
            _send = send;
            _da = da;
            _identityService = identityService;
        }

        public void Send(IEnumerable<IDeliveryJob> jobs)
        {
            var mailouts = jobs
                .GroupBy(x => new
                {
                    x.Email.MessageIdentifier,
                    x.Email.MailoutIdentifier
                })
                .Select(x => new MailoutInfo
                {
                    MessageIdentifier = x.Key.MessageIdentifier.Value,
                    MailoutIdentifier = x.Key.MailoutIdentifier,
                    Deliveries = x
                        .GroupBy(y => y.Language.IfNullOrEmpty(MultilingualString.DefaultLanguage).ToLower())
                        .Select(y => new DeliveryInfo
                        {
                            Language = y.Key,
                            Emails = y.Select(z => z.Email).ToArray()
                        })
                        .ToArray()
                })
                .ToArray();

            StartMailouts(mailouts);
            StartDeliveries(mailouts);
            CompleteMailouts(mailouts);
        }

        private void StartMailouts(IEnumerable<MailoutInfo> mailouts)
        {
            foreach (var mailout in mailouts)
            {
                var start = new StartMailout(mailout.MessageIdentifier, mailout.MailoutIdentifier)
                {
                    OriginOrganization = _identityService.GetCurrentOrganization(),
                    OriginUser = _identityService.GetCurrentUser()
                };

                _send(start);
            }
        }

        private void StartDeliveries(IEnumerable<MailoutInfo> mailouts)
        {
            _logger.Information("Starting deliveries...");

            foreach (var mailout in mailouts)
            {
                if (mailout.Cancelled != null || mailout.Completed != null || DateTimeOffset.UtcNow < mailout.Scheduled)
                    continue;

                foreach (var delivery in mailout.Deliveries)
                {
                    foreach (var message in delivery.Emails)
                    {
                        _send(new StartDeliveries(message.MessageIdentifier.Value, message.MailoutIdentifier, message.Recipients.ToArray())
                        {
                            OriginOrganization = _identityService.GetCurrentOrganization(),
                            OriginUser = _identityService.GetCurrentUser()
                        });
                    }
                }
            }
        }

        private void CompleteMailouts(IEnumerable<MailoutInfo> mailouts)
        {
            foreach (var mailout in mailouts)
            {
                foreach (var delivery in mailout.Deliveries)
                {
                    foreach (var email in delivery.Emails)
                    {
                        CompleteMailoutDelivery(delivery, email);
                    }
                }
            }
        }

        private void CompleteMailoutDelivery(DeliveryInfo delivery, EmailDraft email)
        {
            var ok = false;
            var reason = string.Empty;

            for (int i = 0; i < email.Recipients.Count; i++)
            {
                var recipient = email.Recipients[i];
                var eventNumber = GetValue("EventNumber", recipient.Variables, email.ContentVariables)
                               ?? GetValue("ActivityNumber", recipient.Variables, email.ContentVariables);
                var candidateCode = GetValue("CandidateCode", recipient.Variables, email.ContentVariables);

                if (!eventNumber.HasValue() || !candidateCode.HasValue())
                {
                    reason = $"Direct Access requires a valid numeric individual identification number for the exam candidate, "
                        + "and a valid exam event identification number.";
                    continue;
                }

                if (!int.TryParse(eventNumber, out int eventNumberAsInt))
                {
                    reason = $"Direct Access requires a valid numeric individual ID number for the exam candidate. "
                        + "It will not accept \"{candidateCode}\".";
                    continue;
                }

                var message = MessageHelper.BuildMessage(email, delivery.Language);

                foreach (var variable in recipient.Variables)
                {
                    message.Subject = message.Subject.Replace($"##{variable.Key}##", variable.Value);
                    message.Body = message.Body.Replace($"##{variable.Key}##", variable.Value);
                    message.Body = message.Body.Replace($"-{variable.Key}-", variable.Value);
                }

                var input = new AdHocEventNotificationInput();
                input.EventNumber = eventNumberAsInt;
                input.Recipients.Add(candidateCode);
                input.Subject = message.Subject;
                input.HTML = StringHelper.EncodeBase64(message.Body);
                input.FileName = "None";

                try
                {
                    _da.AdHocEventNotification(email.SenderIdentifier, eventNumber, input);
                    ok = true;
                }
                catch (Exception ex)
                {
                    reason = "An unexpected error occurred when this email notification was submitted to Direct Access. "
                        + CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                }
            }

            if (ok)
            {
                email.MailoutCompleted = DateTimeOffset.UtcNow;
                SendCommand(new CompleteMailout(email.MessageIdentifier.Value, email.MailoutIdentifier));
            }
            else
            {
                email.MailoutCancelled = DateTimeOffset.UtcNow;
                SendCommand(new AbortMailout(email.MessageIdentifier.Value, email.MailoutIdentifier, reason));
            }
        }

        private string GetValue(string name, Dictionary<string, string> recipientVariables, Dictionary<string, string> messageVariables)
        {
            if (recipientVariables.ContainsKey(name) && recipientVariables[name].HasValue())
                return recipientVariables[name];

            if (messageVariables.ContainsKey(name) && messageVariables[name].HasValue())
                return messageVariables[name];

            return null;
        }

        private void SendCommand(ICommand command)
        {
            command.OriginOrganization = _identityService.GetCurrentOrganization();
            command.OriginUser = _identityService.GetCurrentUser();
            _send(command);
        }
    }
}