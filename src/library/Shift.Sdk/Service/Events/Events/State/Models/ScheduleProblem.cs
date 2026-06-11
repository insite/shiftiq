using System;
using System.Collections.Generic;

namespace InSite.Domain.Events
{
    [Serializable]
    public class ScheduleProblem
    {
        public ScheduleProblem()
        {
            SameDayEvents = new List<Guid>();
            SameFormEvents = new List<Guid>();
        }

        public List<Guid> SameDayEvents { get; set; }
        public List<Guid> SameFormEvents { get; set; }
    }
}