using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class Classified : Change
    {
        public Classified(string description, string mailoutType, string recipientType, string recipientRole)
        {
            MessageDescription = description;
            MailoutType = mailoutType;
            RecipientType = recipientType;
            RecipientRole = recipientRole;
        }

        public string MessageDescription { get; set; }
        public string MailoutType { get; set; }
        public string RecipientType { get; set; }
        public string RecipientRole { get; set; }
    }
}