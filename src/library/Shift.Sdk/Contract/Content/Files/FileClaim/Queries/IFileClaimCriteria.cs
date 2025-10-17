using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileClaimCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? FileIdentifier { get; set; }
        Guid? ObjectIdentifier { get; set; }

        string ObjectType { get; set; }

        DateTimeOffset? ClaimGranted { get; set; }
    }
}