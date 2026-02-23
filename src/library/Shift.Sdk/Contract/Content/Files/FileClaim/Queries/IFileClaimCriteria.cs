using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileClaimCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? FileId { get; set; }
        Guid? ObjectId { get; set; }

        string ObjectType { get; set; }

        DateTimeOffset? ClaimGranted { get; set; }
    }
}