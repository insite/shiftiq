using System;

namespace Shift.Contract
{
    public class CreateFileClaim
    {
        public Guid ClaimIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}