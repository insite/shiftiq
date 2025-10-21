using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class MrProgram : IAnnualProgram
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
        public string MR001 { get; set; }
        public string MR002 { get; set; }
        public string MR003 { get; set; }
        public string MR004 { get; set; }
        public string MR005 { get; set; }
        public string MR006 { get; set; }
        public string MR007 { get; set; }
        public string MR008 { get; set; }
        public string MR009 { get; set; }
        public string MR010 { get; set; }
        public string MR011 { get; set; }
        public string MR012 { get; set; }
        public string MR013 { get; set; }
        public string MR014 { get; set; }
        public string MR015 { get; set; }
        public string MR016 { get; set; }
        public string MR017 { get; set; }
        public string MR018 { get; set; }
        public string MR019 { get; set; }
        public string MR020 { get; set; }
        public string MR021 { get; set; }
        public string MR022 { get; set; }
        public string MR023 { get; set; }
        public string MR024 { get; set; }
        public string MR025 { get; set; }
        public string MR026 { get; set; }
        public string MR027 { get; set; }
        public string MR028 { get; set; }
        public string MR029 { get; set; }
        public string MR030 { get; set; }
        public string MR031 { get; set; }
        public string MR032 { get; set; }
        public string MR033 { get; set; }
        public string MR034 { get; set; }
        public string MR035 { get; set; }
        public string MR036 { get; set; }
        public string MR037 { get; set; }
        public string MR038 { get; set; }
        public string MR039 { get; set; }
        public string MR040 { get; set; }
        public string MR041 { get; set; }
        public string MR042 { get; set; }
        public string MR043 { get; set; }
        public string MR044 { get; set; }
        public string MR045 { get; set; }
        public string MR046 { get; set; }
        public string MR047 { get; set; }
        public string MR048 { get; set; }
        public string MR049 { get; set; }
        public string MR050 { get; set; }
        public string MR051 { get; set; }
        public string MR052 { get; set; }
        public string MR053 { get; set; }
        public string MR054 { get; set; }
        public string MR055 { get; set; }
        public string MR056 { get; set; }
        public string MR057 { get; set; }
        public string MR058 { get; set; }
        public string MR059 { get; set; }
        public string MR060 { get; set; }
        public string MR061 { get; set; }
        public string MR062 { get; set; }
        public string MR063 { get; set; }
        public string MR064 { get; set; }
        public string MR065 { get; set; }
        public string MR066 { get; set; }
        public string MR067 { get; set; }
        public string MR068 { get; set; }
        public string MR069 { get; set; }
        public string MR070 { get; set; }
        public string MR071 { get; set; }
        public string MR072 { get; set; }
        public string MR078 { get; set; }
        public string MR079 { get; set; }
        public string MR080 { get; set; }
        public string MR081 { get; set; }
        public string MR082 { get; set; }
        public string MR083 { get; set; }
        public string MR084 { get; set; }
        public string MR085 { get; set; }
        public string MR086 { get; set; }
        public string MR087 { get; set; }
        public string MR088 { get; set; }
        public string MR089 { get; set; }
        public string MR090 { get; set; }
        public string MR091 { get; set; }
        public string MR092 { get; set; }
        public string MR093 { get; set; }
        public string MR094 { get; set; }
        public string MR095 { get; set; }
        public string MR096 { get; set; }
        public string MR097 { get; set; }
        public string MR098 { get; set; }
        public string MR099 { get; set; }
        public string MR100 { get; set; }
        public string MR101 { get; set; }
        public string MR102 { get; set; }
        public string MR103 { get; set; }
        public string MR104 { get; set; }
        public string MR105 { get; set; }
        public string MR106 { get; set; }
        public string MR107 { get; set; }
        public string MR108 { get; set; }
        public string MR109 { get; set; }
        public string MR110 { get; set; }
        public string MR111 { get; set; }
        public string MR112 { get; set; }
        public string MR113 { get; set; }
        public string MR114 { get; set; }
        public string MR115 { get; set; }
        public string MR116 { get; set; }
        public string MR117 { get; set; }
        public string MR118 { get; set; }
        public string MR119 { get; set; }
        public string MR120 { get; set; }
        public string MR121 { get; set; }
        public string MR122 { get; set; }
        public string MR123 { get; set; }
        public string MR124 { get; set; }
        public string MR125 { get; set; }
        public string MR126 { get; set; }
        public string MR127 { get; set; }
        public string MR128 { get; set; }
        public string MR129 { get; set; }
        public string MR130 { get; set; }
        public string MR131 { get; set; }
        public string MR132 { get; set; }
        public string MR133 { get; set; }
        public string MR134 { get; set; }
        public string MR135 { get; set; }
        public string MR136 { get; set; }
        public string MR137 { get; set; }
        public string MR138 { get; set; }
        public string MR139 { get; set; }
        public string MR140 { get; set; }
        public string MR141 { get; set; }
        public string MR142 { get; set; }
        public string MR143 { get; set; }
        public string MR144 { get; set; }
        public string MR145 { get; set; }
        public string MR146 { get; set; }
        public string MR147 { get; set; }
        public string MR148 { get; set; }
        public string MR149 { get; set; }
        public string MR150 { get; set; }
        public string MR151 { get; set; }
        public string MR152 { get; set; }
        public string MR153 { get; set; }
        public string MR154 { get; set; }
        public string MR155 { get; set; }
        public string MR156 { get; set; }
        public string MR157 { get; set; }
        public string MR158 { get; set; }
        public string MR159 { get; set; }
        public string MR160 { get; set; }
        public string MR161 { get; set; }
        public string MR162 { get; set; }
        public string MR163 { get; set; }
        public string MR164 { get; set; }
        public string MR165 { get; set; }
        public string MR166 { get; set; }
        public string MR167 { get; set; }
        public string MR168 { get; set; }
        public string MR169 { get; set; }
        public string MR170 { get; set; }
        public string MR171 { get; set; }
        public string MR172 { get; set; }
        public string MR173 { get; set; }
        public string MR174 { get; set; }
        public string MR175 { get; set; }
        public string MR176 { get; set; }
        public string MR177 { get; set; }
        public string MR178 { get; set; }
        public string MR179 { get; set; }
        public string MR180 { get; set; }
        public string MR181 { get; set; }
        public string MR182 { get; set; }
        public string MR183 { get; set; }
        public string MR184 { get; set; }
        public string MR185 { get; set; }
        public string MR186 { get; set; }
        public string MR187 { get; set; }
        public string RespondentName { get; set; }
        public string StateName { get; set; }
        public string UpdatedBy { get; set; }

        public bool IsVisibleOnTable01 { get; set; }
        public bool IsVisibleOnTable02 { get; set; }
        public bool IsVisibleOnTable03 { get; set; }
        public bool IsVisibleOnTable04 { get; set; }
        public bool IsVisibleOnTable05 { get; set; }
        public bool IsVisibleOnTable06 { get; set; }
        public bool IsVisibleOnTable07 { get; set; }
        public bool IsVisibleOnTable08 { get; set; }
        public bool IsVisibleOnTable09 { get; set; }
        public bool IsVisibleOnTable10 { get; set; }
        public bool IsVisibleOnTable11 { get; set; }
        public bool IsVisibleOnTable12 { get; set; }
        public bool IsVisibleOnTable13 { get; set; }
        public bool IsVisibleOnTable14 { get; set; }
        public bool IsVisibleOnTable15 { get; set; }
        public bool IsVisibleOnTable16 { get; set; }

        public int MrProgramId { get; set; }
        public int? ProgramFolderId { get; set; }
        public int SurveyYear { get; set; }

        public DateTimeOffset? DateTimeSaved { get; set; }
        public DateTimeOffset? DateTimeSubmitted { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public DateTimeOffset? InsertedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }
}