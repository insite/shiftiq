using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class PaProgram : IAnnualProgram
    {
        #region IAnnualProgram

        Guid? IAnnualProgram.AgencyIdentifier
        {
            get => AgencyGroupIdentifier;
            set => AgencyGroupIdentifier = (Guid)value;
        }

        Guid? IAnnualProgram.UserIdentifier
        {
            get => RespondentUserIdentifier;
            set => RespondentUserIdentifier = value;
        }

        #endregion

        public Guid? AgencyGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RespondentUserIdentifier { get; set; }

        public string InsertedBy { get; set; }
        public string PA001 { get; set; }
        public string PA002 { get; set; }
        public string PA003 { get; set; }
        public string PA004 { get; set; }
        public string PA005 { get; set; }
        public string PA006 { get; set; }
        public string PA007 { get; set; }
        public string PA008 { get; set; }
        public string PA009 { get; set; }
        public string PA010 { get; set; }
        public string PA011 { get; set; }
        public string PA012 { get; set; }
        public string PA013 { get; set; }
        public string PA014 { get; set; }
        public string PA015 { get; set; }
        public string PA016 { get; set; }
        public string PA017 { get; set; }
        public string PA018 { get; set; }
        public string PA019 { get; set; }
        public string PA020 { get; set; }
        public string PA021 { get; set; }
        public string PA022 { get; set; }
        public string PA023 { get; set; }
        public string PA024 { get; set; }
        public string PA025 { get; set; }
        public string PA026 { get; set; }
        public string PA027 { get; set; }
        public string PA028 { get; set; }
        public string PA029 { get; set; }
        public string PA030 { get; set; }
        public string PA031 { get; set; }
        public string PA032 { get; set; }
        public string PA033 { get; set; }
        public string PA034 { get; set; }
        public string PA035 { get; set; }
        public string PA036 { get; set; }
        public string PA037 { get; set; }
        public string PA038 { get; set; }
        public string PA039 { get; set; }
        public string PA040 { get; set; }
        public string PA041 { get; set; }
        public string PA042 { get; set; }
        public string PA043 { get; set; }
        public string PA044 { get; set; }
        public string PA045 { get; set; }
        public string PA046 { get; set; }
        public string PA047 { get; set; }
        public string PA048 { get; set; }
        public string PA049 { get; set; }
        public string PA050 { get; set; }
        public string PA051 { get; set; }
        public string RespondentName { get; set; }
        public string StateName { get; set; }
        public string UpdatedBy { get; set; }

        public bool IsVisibleOnTable01 { get; set; }
        public bool IsVisibleOnTable02 { get; set; }
        public bool IsVisibleOnTable03 { get; set; }
        public bool IsVisibleOnTable04 { get; set; }

        public int PaProgramId { get; set; }
        public int? ProgramFolderId { get; set; }
        public int SurveyYear { get; set; }

        public DateTimeOffset? DateTimeSaved { get; set; }
        public DateTimeOffset? DateTimeSubmitted { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public DateTimeOffset? InsertedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }
}