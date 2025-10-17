using System;

namespace Shift.Toolbox.Integration.DirectAccess
{
    [Serializable]
    public class VerificationDisplayVariables
    {
        public string CandidateName { get; set; }

        public string CandidateStatusReason { get; set; }

        public string ClassCode { get; set; }

        public DateTime? ClassEndDate { get; set; }

        public string ClassStatusReason { get; set; }

        public string EventType { get; set; }

        public string TradeStatusReason { get; set; }
    }
}