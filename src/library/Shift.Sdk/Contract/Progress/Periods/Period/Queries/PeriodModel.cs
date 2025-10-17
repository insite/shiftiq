using System;

namespace Shift.Contract
{
    public partial class PeriodModel
    {
        public Guid PeriodId { get; set; }

        public string PeriodName { get; set; }

        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
    }
}