using System.Collections.Generic;

using Shift.Constant;

namespace Shift.Common
{
    public class MailgunStatus
    {
        public string Status { get; private set; }
        public string Description { get; set; }
        public Dictionary<string, string> Data { get; } = new Dictionary<string, string>();

        private MailgunStatus() { }

        public static MailgunStatus Reject(string reason)
        {
            return new MailgunStatus
            {
                Status = MailoutCallbackStatus.Rejected,
                Description = reason
            };
        }

        public static MailgunStatus Queue()
        {
            return new MailgunStatus
            {
                Status = MailoutCallbackStatus.Queued
            };
        }
    }
}
