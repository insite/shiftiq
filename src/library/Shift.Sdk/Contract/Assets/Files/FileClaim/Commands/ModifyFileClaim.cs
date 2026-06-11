using System;

namespace Shift.Contract
{
    public class ModifyFileClaim
    {
        public Guid ClaimId { get; set; }
        public Guid FileId { get; set; }
        public Guid ObjectId { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}