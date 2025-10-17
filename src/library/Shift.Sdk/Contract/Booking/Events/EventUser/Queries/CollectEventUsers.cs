using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectEventUsers : Query<IEnumerable<EventUserModel>>, IEventUserCriteria
    {
        public Guid? EventId { get; set; }
        public Guid? UserId { get; set; }

        public string Role { get; set; }
    }
}