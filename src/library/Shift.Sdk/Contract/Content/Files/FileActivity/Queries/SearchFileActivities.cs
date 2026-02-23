using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFileActivities : Query<IEnumerable<FileActivityMatch>>, IFileActivityCriteria
    {
        public Guid? FileId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string ActivityChanges { get; set; }

        public DateTimeOffset? ActivityTime { get; set; }
    }
}