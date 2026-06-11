using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? ObjectId { get; set; }
        Guid? UserId { get; set; }
        Guid[] FileIds { get; set; }

        string ObjectTypeExact { get; set; }
        string ObjectIdentifierContains { get; set; }
        string FileNameContains { get; set; }
        string DocumentNameContains { get; set; }

        DateTimeOffset? FileUploadedSince { get; set; }
        DateTimeOffset? FileUploadedBefore { get; set; }

        bool? HasClaims { get; set; }
    }
}