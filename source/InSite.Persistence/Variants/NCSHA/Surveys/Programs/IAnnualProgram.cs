using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public interface IAnnualProgram
    {
        int SurveyYear { get; set; }
        string StateName { get; set; }
        DateTimeOffset? Deadline { get; set; }
        DateTimeOffset? DateTimeSaved { get; set; }
        DateTimeOffset? DateTimeSubmitted { get; set; }

        Guid? AgencyIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string InsertedBy { get; set; }
        DateTimeOffset? InsertedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTimeOffset? UpdatedOn { get; set; }
    }
}
