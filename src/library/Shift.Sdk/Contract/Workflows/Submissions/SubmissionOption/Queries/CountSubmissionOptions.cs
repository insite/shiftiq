using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissionOptions : Query<int>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
