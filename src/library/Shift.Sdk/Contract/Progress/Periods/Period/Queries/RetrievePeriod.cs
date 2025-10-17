using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePeriod : Query<PeriodModel>
    {
        public Guid PeriodIdentifier { get; set; }
    }
}