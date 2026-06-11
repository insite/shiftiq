using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAssessment : Query<bool>
    {
        public Guid FormIdentifier { get; set; }
    }
}