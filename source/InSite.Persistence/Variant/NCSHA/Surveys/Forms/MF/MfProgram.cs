using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class MfProgram : IAnnualProgram
    {
        #region IAnnualProgram

        Guid? IAnnualProgram.AgencyIdentifier
        {
            get => AgencyGroupIdentifier;
            set => AgencyGroupIdentifier = value;
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
        public string MF001 { get; set; }
        public string MF002 { get; set; }
        public string MF003 { get; set; }
        public string MF004 { get; set; }
        public string MF005 { get; set; }
        public string MF006 { get; set; }
        public string MF007 { get; set; }
        public string MF008 { get; set; }
        public string MF009 { get; set; }
        public string MF010 { get; set; }
        public string MF011 { get; set; }
        public string MF012 { get; set; }
        public string MF013 { get; set; }
        public string MF014 { get; set; }
        public string MF015 { get; set; }
        public string MF016 { get; set; }
        public string MF017 { get; set; }
        public string MF018 { get; set; }
        public string MF019 { get; set; }
        public string MF020 { get; set; }
        public string MF021 { get; set; }
        public string MF022 { get; set; }
        public string MF023 { get; set; }
        public string MF024 { get; set; }
        public string MF025 { get; set; }
        public string MF026 { get; set; }
        public string MF027 { get; set; }
        public string MF028 { get; set; }
        public string MF029 { get; set; }
        public string MF030 { get; set; }
        public string MF031 { get; set; }
        public string MF032 { get; set; }
        public string MF033 { get; set; }
        public string MF034 { get; set; }
        public string MF035 { get; set; }
        public string MF036 { get; set; }
        public string MF037 { get; set; }
        public string MF038 { get; set; }
        public string MF039 { get; set; }
        public string MF040 { get; set; }
        public string MF041 { get; set; }
        public string MF042 { get; set; }
        public string MF043 { get; set; }
        public string MF044 { get; set; }
        public string MF045 { get; set; }
        public string MF046 { get; set; }
        public string MF047 { get; set; }
        public string MF048 { get; set; }
        public string MF049 { get; set; }
        public string MF050 { get; set; }
        public string MF051 { get; set; }
        public string MF052 { get; set; }
        public string MF053 { get; set; }
        public string MF054 { get; set; }
        public string MF055 { get; set; }
        public string MF057 { get; set; }
        public string MF058 { get; set; }
        public string MF059 { get; set; }
        public string MF060 { get; set; }
        public string MF061 { get; set; }
        public string MF062 { get; set; }
        public string MF063 { get; set; }
        public string MF064 { get; set; }
        public string MF065 { get; set; }
        public string MF066 { get; set; }
        public string MF067 { get; set; }
        public string MF068 { get; set; }
        public string MF069 { get; set; }
        public string MF070 { get; set; }
        public string MF071 { get; set; }
        public string MF072 { get; set; }
        public string MF073 { get; set; }
        public string MF074 { get; set; }
        public string MF075 { get; set; }
        public string MF076 { get; set; }
        public string MF077 { get; set; }
        public string MF078 { get; set; }
        public string MF079 { get; set; }
        public string MF080 { get; set; }
        public string MF081 { get; set; }
        public string MF082 { get; set; }
        public string MF083 { get; set; }
        public string MF084 { get; set; }
        public string MF085 { get; set; }
        public string MF086 { get; set; }
        public string MF087 { get; set; }
        public string MF088 { get; set; }
        public string MF089 { get; set; }
        public string MF090 { get; set; }
        public string MF092 { get; set; }
        public string MF095 { get; set; }
        public string MF096 { get; set; }
        public string MF097 { get; set; }
        public string MF098 { get; set; }
        public string MF099 { get; set; }
        public string MF100 { get; set; }
        public string MF101 { get; set; }
        public string MF102 { get; set; }
        public string MF103 { get; set; }
        public string MF104 { get; set; }
        public string MF105 { get; set; }
        public string MF106 { get; set; }
        public string MF107 { get; set; }
        public string MF108 { get; set; }
        public string MF109 { get; set; }
        public string MF110 { get; set; }
        public string MF111 { get; set; }
        public string MF112 { get; set; }
        public string MF113 { get; set; }
        public string MF114 { get; set; }
        public string MF115 { get; set; }
        public string MF116 { get; set; }
        public string MF117 { get; set; }
        public string MF118 { get; set; }
        public string MF119 { get; set; }
        public string MF120 { get; set; }
        public string MF121 { get; set; }
        public string MF122 { get; set; }
        public string MF123 { get; set; }
        public string MF124 { get; set; }
        public string MF125 { get; set; }
        public string MF126 { get; set; }
        public string MF127 { get; set; }
        public string MF128 { get; set; }
        public string MF129 { get; set; }
        public string MF130 { get; set; }
        public string MF131 { get; set; }
        public string MF132 { get; set; }
        public string MF133 { get; set; }
        public string MF134 { get; set; }
        public string MF135 { get; set; }
        public string MF136 { get; set; }
        public string MF137 { get; set; }
        public string MF138 { get; set; }
        public string MF139 { get; set; }
        public string MF140 { get; set; }
        public string MF141 { get; set; }
        public string MF142 { get; set; }
        public string MF143 { get; set; }
        public string MF144 { get; set; }
        public string MF145 { get; set; }
        public string MF146 { get; set; }
        public string MF147 { get; set; }
        public string MF148 { get; set; }
        public string MF149 { get; set; }
        public string MF150 { get; set; }
        public string MF151 { get; set; }
        public string MF152 { get; set; }
        public string MF153 { get; set; }
        public string MF154 { get; set; }
        public string MF155 { get; set; }
        public string MF156 { get; set; }
        public string MF177 { get; set; }
        public string MF182 { get; set; }
        public string MF183 { get; set; }
        public string MF184 { get; set; }
        public string MF185 { get; set; }
        public string MF186 { get; set; }
        public string MF187 { get; set; }
        public string MF188 { get; set; }
        public string MF189 { get; set; }
        public string MF190 { get; set; }
        public string MF191 { get; set; }
        public string MF192 { get; set; }
        public string MF193 { get; set; }
        public string MF194 { get; set; }
        public string MF195 { get; set; }
        public string MF196 { get; set; }
        public string MF197 { get; set; }
        public string MF198 { get; set; }
        public string MF199 { get; set; }
        public string MF200 { get; set; }
        public string MF201 { get; set; }
        public string MF202 { get; set; }
        public string MF203 { get; set; }
        public string MF204 { get; set; }
        public string MF205 { get; set; }
        public string MF206 { get; set; }
        public string MF207 { get; set; }
        public string MF208 { get; set; }
        public string MF209 { get; set; }
        public string MF210 { get; set; }
        public string MF211 { get; set; }
        public string MF212 { get; set; }
        public string MF214 { get; set; }
        public string MF215 { get; set; }
        public string MF216 { get; set; }
        public string MF217 { get; set; }
        public string MF218 { get; set; }
        public string MF219 { get; set; }
        public string MF220 { get; set; }
        public string MF221 { get; set; }
        public string MF222 { get; set; }
        public string MF223 { get; set; }
        public string MF224 { get; set; }
        public string MF225 { get; set; }
        public string MF226 { get; set; }
        public string MF227 { get; set; }
        public string MF228 { get; set; }
        public string MF229 { get; set; }
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

        public int MfProgramId { get; set; }
        public int? ProgramFolderId { get; set; }
        public int SurveyYear { get; set; }

        public DateTimeOffset? DateTimeSaved { get; set; }
        public DateTimeOffset? DateTimeSubmitted { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public DateTimeOffset? InsertedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }
}