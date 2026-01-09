using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Obsolete]
    public class CarbonCopyCompleted : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public EmailAddress Recipient { get; set; }
        public string CcType { get; set; }
        public EmailAddress Cc { get; set; }
        public string Error { get; set; }

        public CarbonCopyCompleted(Guid mailout, EmailAddress recipient, string ccType, EmailAddress cc, string error)
        {
            MailoutIdentifier = mailout;
            Recipient = recipient;
            CcType = ccType;
            Cc = cc;
            Error = error;
        }
    }

    public class CarbonCopyCompleted2 : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string CcType { get; set; }
        public Guid CcIdentifier { get; set; }
        public string Error { get; set; }

        public CarbonCopyCompleted2(Guid mailout, Guid recipient, string ccType, Guid cc, string error)
        {
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
            CcType = ccType;
            CcIdentifier = cc;
            Error = error;
        }
    }
}