using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Obsolete]
    public class CarbonCopyStarted : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public EmailAddress Recipient { get; set; }
        public String CcType { get; set; }
        public EmailAddress Cc { get; set; }

        public CarbonCopyStarted(Guid mailout, EmailAddress recipient, string ccType, EmailAddress cc)
        {
            MailoutIdentifier = mailout;
            Recipient = recipient;
            CcType = ccType;
            Cc = cc;
        }
    }

    public class CarbonCopyStarted2 : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public string CcType { get; set; }
        public Guid CcIdentifier { get; set; }

        public CarbonCopyStarted2(Guid mailout, Guid recipient, string ccType, Guid cc)
        {
            MailoutIdentifier = mailout;
            RecipientIdentifier = recipient;
            CcType = ccType;
            CcIdentifier = cc;
        }
    }
}