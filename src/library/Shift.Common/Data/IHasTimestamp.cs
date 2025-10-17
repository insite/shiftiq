using System;

namespace Shift.Common
{
    public interface IHasTimestamp
    {
        Guid CreatedBy { get; set; }
        DateTimeOffset Created { get; set; }
        Guid ModifiedBy { get; set; }
        DateTimeOffset Modified { get; set; }
    }
}
