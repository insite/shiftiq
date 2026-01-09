using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountEventUsers : Query<int>, IEventUserCriteria
    {
        public Guid? EventId { get; set; }
        public Guid? UserId { get; set; }

        public string Role { get; set; }
    }
}