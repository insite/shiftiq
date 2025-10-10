using System;

namespace Shift.Contract.Requests
{
    public class ComplianceSummaryRequest
    {
        public Guid[] Departments { get; set; }
        public Guid[] Learners { get; set; }
        public int[] Measurements { get; set; }
        public int ProfileCondition { get; set; }
        public bool LearnerMustHaveProfile { get; set; }
    }
}