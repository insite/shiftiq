using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shift.Common
{
    public class MailgunServerSettings
    {
        public bool EmailOutboxDisabled { get; }
        public bool EmailOutboxFiltered { get; }
        public ReadOnlyCollection<string> WhitelistDomains { get; }
        public ReadOnlyCollection<string> WhitelistTesters { get; }
        public ReadOnlyCollection<string> ForcedTypes { get; }
        public string Domain { get; }

        public int MinutesBeforeCancellationIsDisallowed { get; set; } = 5;

        public bool MailgunCallbackEnabled { get; }

        public MailgunServerSettings(IPartitionModel partition, EnvironmentModel environment, Application application)
        {
            EmailOutboxDisabled = application.EmailOutboxDisabled;
            EmailOutboxFiltered = application.EmailOutboxFiltered;
            ForcedTypes = application.AlertsToForceSendList.EmptyIfNull().AsReadOnly();

            WhitelistDomains = Array.AsReadOnly(StringHelper.Split(partition.WhitelistDomains));
            WhitelistTesters = Array.AsReadOnly(StringHelper.Split(partition.WhitelistEmails));

            Domain = (environment.GetSubdomainPrefix() + partition.Slug + "." + partition.Domain).ToLowerInvariant();

            MailgunCallbackEnabled = application.MailgunCallbackEnabled;
        }

        public MailgunServerSettings(
            bool emailOutboxDisabled, bool emailOutboxFiltered,
            IEnumerable<string> whitelistDomains, IEnumerable<string> whitelistTesters, IEnumerable<string> forcedTypes,
            string domain)
        {
            EmailOutboxDisabled = emailOutboxDisabled;
            EmailOutboxFiltered = emailOutboxFiltered;
            WhitelistDomains = Array.AsReadOnly(whitelistDomains.EmptyIfNull().ToArray());
            WhitelistTesters = Array.AsReadOnly(whitelistTesters.EmptyIfNull().ToArray());
            ForcedTypes = Array.AsReadOnly(forcedTypes.EmptyIfNull().ToArray());
            Domain = domain;
        }
    }
}
