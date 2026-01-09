using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }
        Guid? ObjectIdentifier { get; set; }

        string ObjectTypeExact { get; set; }
        string ObjectIdentifierContains { get; set; }
        string FileNameContains { get; set; }
        string DocumentNameContains { get; set; }

        DateTimeOffset? FileUploadedSince { get; set; }
        DateTimeOffset? FileUploadedBefore { get; set; }

        bool? HasClaims { get; set; }
    }
}