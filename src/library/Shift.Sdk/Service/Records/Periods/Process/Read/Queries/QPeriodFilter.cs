using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QPeriodFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string ExactPeriodName { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset? PeriodSince { get; set; }
        public DateTimeOffset? PeriodBefore { get; set; }
        public HashSet<Guid> Identifiers { get; set; }
        public HashSet<Guid> ExcludeIdentifiers { get; set; }

        public QPeriodFilter Clone()
        {
            return (QPeriodFilter)MemberwiseClone();
        }
    }
}
