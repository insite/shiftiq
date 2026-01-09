using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class MfProgramFilter : Shift.Common.Filter
    {
        public string AgencyName { get; set; }
        public int? SurveyYear { get; set; }
        public DateTime? DateTimeSavedSince { get; set; }
        public DateTime? DateTimeSavedBefore { get; set; }
    }
}