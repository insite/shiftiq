using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFileClaims : Query<IEnumerable<FileClaimModel>>, IFileClaimCriteria
    {
        public Guid? FileId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}