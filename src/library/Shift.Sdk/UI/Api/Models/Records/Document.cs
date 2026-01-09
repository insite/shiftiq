using System;

namespace Shift.Sdk.UI
{
    public class Document
    {
        public Guid Identifier { get; set; }
        public Guid Template { get; set; }
        public string Status { get; set; }
        public int? Score { get; set; }
        public long Granted { get; set; } = 0;
        public long Expiry { get; set; } = 0;

        public Learner Learner { get; set; }
        public string CourseName { get; set; }

        public Document()
        {
            Learner = new Learner();
        }
    }
}