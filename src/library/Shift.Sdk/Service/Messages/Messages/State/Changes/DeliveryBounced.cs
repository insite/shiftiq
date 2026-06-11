using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class DeliveryBounced : Change
    {
        public string BounceFile { get; set; }
        
        public DateTimeOffset BounceTime { get; set; }
        public string BounceType { get; set; }
        public string BounceReason { get; set; }
        public string BounceSubject { get; set; }
        public string BounceBody { get; set; }
        
        public string RecipientAddress { get; set; }
        
        public Guid? MailoutIdentifier { get; set; }

        public DeliveryBounced(string file, DateTimeOffset time, string type, string reason, string subject, string body, string address, Guid? mailout)
        {
            BounceFile = file;
            BounceTime = time;
            BounceType = type;
            BounceReason = reason;
            BounceSubject = subject;
            BounceBody = body;
            RecipientAddress = address;
            MailoutIdentifier = mailout;
        }
    }
}