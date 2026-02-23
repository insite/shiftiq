using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFileActivities : Query<IEnumerable<FileActivityModel>>, IFileActivityCriteria
    {
        public Guid? FileId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}