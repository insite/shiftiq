using System;

namespace Shift.Contract
{
    public class CaseStatusMatch
    {
        public Guid StatusIdentifier { get; set; }
        public string StatusName { get; set; }
        public string CaseType { get; set; }
        public string StatusCategory { get; set; }
        public int StatusSequence { get; set; }
    }
}
