using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchSubmissionOptions : Query<IEnumerable<SubmissionOptionMatch>>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
