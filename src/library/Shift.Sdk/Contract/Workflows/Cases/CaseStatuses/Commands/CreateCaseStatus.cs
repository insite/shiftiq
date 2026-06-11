using System;

namespace Shift.Contract
{
    public class CreateCaseStatus
    {
        public string CaseType { get; set; }
        public string StatusName { get; set; }
        public int StatusSequence { get; set; }
        public string StatusCategory { get; set; }
        public string ReportCategory { get; set; }
        public string StatusDescription { get; set; }
    }
}
