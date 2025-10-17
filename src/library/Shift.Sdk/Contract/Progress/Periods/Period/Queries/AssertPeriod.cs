using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPeriod : Query<bool>
    {
        public Guid PeriodIdentifier { get; set; }
    }
}