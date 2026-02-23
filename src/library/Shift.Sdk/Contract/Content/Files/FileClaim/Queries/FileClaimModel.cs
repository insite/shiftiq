using System;

namespace Shift.Contract
{
    public partial class FileClaimModel
    {
        public Guid ClaimId { get; set; }
        public Guid FileId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid OrganizationId { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}