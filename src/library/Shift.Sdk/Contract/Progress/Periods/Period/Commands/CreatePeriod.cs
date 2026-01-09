using System;

namespace Shift.Contract
{
    public class CreatePeriod
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid PeriodIdentifier { get; set; }

        public string PeriodName { get; set; }

        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
    }
}