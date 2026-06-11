using System;

namespace Shift.Contract
{
    public class ModifyPeriod
    {
        public Guid OrganizationId { get; set; }
        public Guid PeriodId { get; set; }

        public string PeriodName { get; set; }

        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
    }
}