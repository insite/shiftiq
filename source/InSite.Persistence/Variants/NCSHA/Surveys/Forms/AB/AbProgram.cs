using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class AbProgram : IAnnualProgram
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

        public string AB001 { get; set; }
        public string AB002 { get; set; }
        public string AB003 { get; set; }
        public string AB004 { get; set; }
        public string AB005 { get; set; }
        public string AB006 { get; set; }
        public string AB007 { get; set; }
        public string AB008 { get; set; }
        public string AB009 { get; set; }
        public string AB010 { get; set; }
        public string AB011 { get; set; }
        public string AB012 { get; set; }
        public string AB013 { get; set; }
        public string AB014 { get; set; }
        public string AB015 { get; set; }
        public string AB016 { get; set; }
        public string AB017 { get; set; }
        public string AB018 { get; set; }
        public string AB019 { get; set; }
        public string AB020 { get; set; }
        public string AB021 { get; set; }
        public string AB022 { get; set; }
        public string AB023 { get; set; }
        public string AB024 { get; set; }
        public string AB025 { get; set; }
        public string AB026 { get; set; }
        public string AB027 { get; set; }
        public string AB028 { get; set; }
        public string AB029 { get; set; }
        public string AB030 { get; set; }
        public string AB031 { get; set; }
        public string AB032 { get; set; }
        public string AB033 { get; set; }
        public string AB034 { get; set; }
        public string AB035 { get; set; }
        public string AB036 { get; set; }
        public string AB037 { get; set; }
        public string AB038 { get; set; }
        public string AB039 { get; set; }
        public string AB040 { get; set; }
        public string AB041 { get; set; }
        public string AB042 { get; set; }
        public string AB043 { get; set; }
        public string AB044 { get; set; }
        public string AB045 { get; set; }
        public string AB046 { get; set; }
        public string AB047 { get; set; }
        public string AB048 { get; set; }
        public string AB049 { get; set; }
        public string AB050 { get; set; }
        public string AB051 { get; set; }
        public string AB052 { get; set; }
        public string AB053 { get; set; }
        public string AB054 { get; set; }
        public string AB055 { get; set; }
        public string AB056 { get; set; }
        public string AB057 { get; set; }
        public string AB058 { get; set; }
        public string AB059 { get; set; }
        public string AB060 { get; set; }
        public string AB061 { get; set; }
        public string AB062 { get; set; }
        public string AB063 { get; set; }
        public string AB064 { get; set; }
        public string AB065 { get; set; }
        public string AB066 { get; set; }
        public string AB067 { get; set; }
        public string AB068 { get; set; }
        public string AB069 { get; set; }
        public string AB070 { get; set; }
        public string AB071 { get; set; }
        public string AB072 { get; set; }
        public string AB073 { get; set; }
        public string AB074 { get; set; }
        public string AB075 { get; set; }
        public string AB076 { get; set; }
        public string AB077 { get; set; }
        public string AB078 { get; set; }
        public string AB079 { get; set; }
        public string AB080 { get; set; }
        public string AB081 { get; set; }
        public string AB082 { get; set; }
        public string AB083 { get; set; }
        public string AB084 { get; set; }
        public string AB085 { get; set; }
        public string AB086 { get; set; }
        public string AB087 { get; set; }
        public string AB088 { get; set; }
        public string AB089 { get; set; }
        public string AB090 { get; set; }
        public string AB091 { get; set; }
        public string AB092 { get; set; }
        public string AB093 { get; set; }
        public string AB094 { get; set; }
        public string AB095 { get; set; }
        public string AB096 { get; set; }
        public string AB097 { get; set; }
        public string AB098 { get; set; }
        public string AB099 { get; set; }
        public string AB100 { get; set; }
        public string AB101 { get; set; }
        public string AB102 { get; set; }
        public string AB103 { get; set; }
        public string AB104 { get; set; }
        public string AB105 { get; set; }
        public string AB106 { get; set; }
        public string AB107 { get; set; }
        public string AB108 { get; set; }
        public string AB109 { get; set; }
        public string AB110 { get; set; }
        public string AB111 { get; set; }
        public string AB112 { get; set; }
        public string AB113 { get; set; }
        public string AB114 { get; set; }
        public string AB115 { get; set; }
        public string AB116 { get; set; }
        public string AB117 { get; set; }
        public string AB118 { get; set; }
        public string AB119 { get; set; }
        public string AB120 { get; set; }
        public string AB121 { get; set; }
        public string AB122 { get; set; }
        public string AB123 { get; set; }
        public string AB124 { get; set; }
        public string AB125 { get; set; }
        public string AB126 { get; set; }
        public string AB127 { get; set; }
        public string AB128 { get; set; }
        public string AB129 { get; set; }
        public string AB130 { get; set; }
        public string AB131 { get; set; }
        public string AB132 { get; set; }
        public string AB133 { get; set; }
        public string AB134 { get; set; }
        public string AB135 { get; set; }
        public string AB136 { get; set; }
        public string AB137 { get; set; }
        public string AB138 { get; set; }
        public string AB139 { get; set; }
        public string AB140 { get; set; }
        public string AB141 { get; set; }
        public string AB142 { get; set; }
        public string AB143 { get; set; }
        public string AB144 { get; set; }
        public string AB145 { get; set; }
        public string AB146 { get; set; }
        public string AB147 { get; set; }
        public string AB148 { get; set; }
        public string AB149 { get; set; }
        public string AB150 { get; set; }
        public string AB151 { get; set; }
        public string AB152 { get; set; }
        public string AB153 { get; set; }
        public string AB154 { get; set; }
        public string AB155 { get; set; }
        public string AB156 { get; set; }
        public string AB157 { get; set; }
        public string AB158 { get; set; }
        public string AB159 { get; set; }
        public string AB160 { get; set; }
        public string AB161 { get; set; }
        public string AB162 { get; set; }
        public string AB163 { get; set; }
        public string AB164 { get; set; }
        public string AB165 { get; set; }
        public string AB166 { get; set; }
        public string AB167 { get; set; }
        public string AB168 { get; set; }
        public string AB169 { get; set; }
        public string AB170 { get; set; }
        public string AB171 { get; set; }
        public string AB172 { get; set; }
        public string AB173 { get; set; }
        public string AB174 { get; set; }
        public string AB175 { get; set; }
        public string AB176 { get; set; }
        public string InsertedBy { get; set; }
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

        public int AbProgramId { get; set; }
        public int? ProgramFolderId { get; set; }
        public int SurveyYear { get; set; }

        public DateTimeOffset? DateTimeSaved { get; set; }
        public DateTimeOffset? DateTimeSubmitted { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public DateTimeOffset? InsertedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }
}