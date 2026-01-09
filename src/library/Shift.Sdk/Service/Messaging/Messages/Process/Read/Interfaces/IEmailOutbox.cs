using System;
using System.Collections.Specialized;

using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Messages.Read
{
    public interface IEmailOutbox
    {
        EmailDraft[] Compose(
            EnvironmentName environment,
            NotificationType trigger,
            Guid organization,
            Guid user,
            Guid? recipient,
            Guid? message,
            StringDictionary variables,
            Guid[] to = null, Guid[] cc = null, Guid[] bcc = null
        );

        void Send(EmailDraft email, string tag, bool isUnitTest = false, string type = null);

        void SendAndReplacePlaceholders(EmailDraft email, string tag);

        void Send(EmailDraft email);
    }
}