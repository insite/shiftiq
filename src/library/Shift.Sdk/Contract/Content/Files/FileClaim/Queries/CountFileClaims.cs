using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFileClaims : Query<int>, IFileClaimCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}