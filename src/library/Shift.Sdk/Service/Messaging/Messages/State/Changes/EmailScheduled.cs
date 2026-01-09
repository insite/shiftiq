using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public class EmailScheduled : Change
    {
        public EmailScheduled(EmailDraft email)
        {
            Email = email;
        }

        public EmailDraft Email { get; set; }
    }
}