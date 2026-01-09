using System;
using System.Collections.Generic;

namespace Shift.Toolbox.Integration.DirectAccess
{
    [Serializable]
    public class VerificationInputVariables
    {
        public string CandidateCode { get; set; }
        public string CandidateStatus { get; set; }
        public List<string> ClassFormCodes { get; set; }
        public string ClassStatus { get; set; }
        public DateTimeOffset CurrentTime { get; set; }
        public bool AdminIsInternal { get; set; }
        public bool EventIsOnline { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public string FormCode { get; set; }
        public List<DateTimeOffset> HolidayCalendar { get; set; }
        public int ScheduleConflictCount { get; set; }
        public int FormConflictCount { get; set; }
        public List<CodeAndDate> ScheduledExams { get; set; }
        public DateTimeOffset? TradeLevelAttempted { get; set; }
        public string TradeStatus { get; set; }

        public VerificationInputVariables()
        {
            AdminIsInternal = true;
            ClassFormCodes = new List<string>();
            HolidayCalendar = new List<DateTimeOffset>();
            ScheduledExams = new List<CodeAndDate>();
        }
    }
}