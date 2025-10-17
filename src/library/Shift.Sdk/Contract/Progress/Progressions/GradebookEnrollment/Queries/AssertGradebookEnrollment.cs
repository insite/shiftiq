using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGradebookEnrollment : Query<bool>
    {
        public Guid EnrollmentIdentifier { get; set; }
    }
}