using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAssessment : Query<AssessmentModel>
    {
        public Guid FormIdentifier { get; set; }
    }
}