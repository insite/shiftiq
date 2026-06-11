using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountStandards : Query<int>, IStandardCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? ParentStandardId { get; set; }
        public Guid[] ParentStandardIds { get; set; }
        public Guid[] StandardIds { get; set; }

        public string ContentTitle { get; set; }
        public string StandardType { get; set; }
    }
}
