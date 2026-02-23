using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFileActivities : Query<int>, IFileActivityCriteria
    {
        public Guid? FileId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}