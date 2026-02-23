using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileActivityCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? FileId { get; set; }
        Guid? UserId { get; set; }

        string ActivityChanges { get; set; }

        DateTimeOffset? ActivityTime { get; set; }
    }
}