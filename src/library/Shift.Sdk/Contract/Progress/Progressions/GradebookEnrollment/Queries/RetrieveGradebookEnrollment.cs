using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGradebookEnrollment : Query<GradebookEnrollmentModel>
    {
        public Guid EnrollmentIdentifier { get; set; }
    }
}