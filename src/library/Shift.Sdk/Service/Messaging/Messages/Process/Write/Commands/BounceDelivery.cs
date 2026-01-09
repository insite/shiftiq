using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class BounceDelivery : Command
    {
        public Guid BounceIdentifier { get; set; }
        
        public DateTimeOffset BounceTime { get; set; }
        public string BounceFile { get; set; }
        public string BounceType { get; set; }
        public string BounceReason { get; set; }
        public string BounceSubject { get; set; }
        public string BounceBody { get; set; }
        
        public string RecipientAddress { get; set; }
        
        public Guid? MailoutIdentifier { get; set; }

        public BounceDelivery(Guid bounce, string file, DateTimeOffset time, string type, string reason, string subject, string body, string address, Guid? mailout)
        {
            BounceIdentifier = bounce;
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