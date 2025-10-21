using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class HiProgram : IAnnualProgram
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

        public string HI001 { get; set; }
        public string HI002 { get; set; }
        public string HI003 { get; set; }
        public string HI004 { get; set; }
        public string HI005 { get; set; }
        public string HI006 { get; set; }
        public string HI007 { get; set; }
        public string HI008 { get; set; }
        public string HI009 { get; set; }
        public string HI010 { get; set; }
        public string HI011 { get; set; }
        public string HI012 { get; set; }
        public string HI013 { get; set; }
        public string HI014 { get; set; }
        public string HI015 { get; set; }
        public string HI016 { get; set; }
        public string HI017 { get; set; }
        public string HI018 { get; set; }
        public string HI019 { get; set; }
        public string HI020 { get; set; }
        public string HI021 { get; set; }
        public string HI022 { get; set; }
        public string HI023 { get; set; }
        public string HI024 { get; set; }
        public string HI025 { get; set; }
        public string HI026 { get; set; }
        public string HI027 { get; set; }
        public string HI028 { get; set; }
        public string HI029 { get; set; }
        public string HI030 { get; set; }
        public string HI031 { get; set; }
        public string HI032 { get; set; }
        public string HI033 { get; set; }
        public string HI034 { get; set; }
        public string HI035 { get; set; }
        public string HI036 { get; set; }
        public string HI037 { get; set; }
        public string HI038 { get; set; }
        public string HI039 { get; set; }
        public string HI040 { get; set; }
        public string HI041 { get; set; }
        public string HI042 { get; set; }
        public string HI043 { get; set; }
        public string HI044 { get; set; }
        public string HI045 { get; set; }
        public string HI046 { get; set; }
        public string HI047 { get; set; }
        public string HI048 { get; set; }
        public string HI049 { get; set; }
        public string HI050 { get; set; }
        public string HI051 { get; set; }
        public string HI052 { get; set; }
        public string HI053 { get; set; }
        public string HI054 { get; set; }
        public string HI055 { get; set; }
        public string HI056 { get; set; }
        public string HI057 { get; set; }
        public string HI058 { get; set; }
        public string HI059 { get; set; }
        public string HI060 { get; set; }
        public string HI061 { get; set; }
        public string HI062 { get; set; }
        public string HI063 { get; set; }
        public string HI064 { get; set; }
        public string HI065 { get; set; }
        public string HI066 { get; set; }
        public string HI067 { get; set; }
        public string HI068 { get; set; }
        public string HI069 { get; set; }
        public string HI070 { get; set; }
        public string HI071 { get; set; }
        public string HI072 { get; set; }
        public string HI073 { get; set; }
        public string HI074 { get; set; }
        public string HI075 { get; set; }
        public string HI076 { get; set; }
        public string HI077 { get; set; }
        public string HI078 { get; set; }
        public string HI079 { get; set; }
        public string HI080 { get; set; }
        public string HI081 { get; set; }
        public string HI082 { get; set; }
        public string HI083 { get; set; }
        public string HI084 { get; set; }
        public string HI085 { get; set; }
        public string HI086 { get; set; }
        public string HI087 { get; set; }
        public string HI088 { get; set; }
        public string HI089 { get; set; }
        public string HI090 { get; set; }
        public string HI091 { get; set; }
        public string HI092 { get; set; }
        public string HI093 { get; set; }
        public string HI094 { get; set; }
        public string HI095 { get; set; }
        public string HI096 { get; set; }
        public string HI097 { get; set; }
        public string HI098 { get; set; }
        public string HI099 { get; set; }
        public string HI100 { get; set; }
        public string HI101 { get; set; }
        public string HI102 { get; set; }
        public string HI103 { get; set; }
        public string HI104 { get; set; }
        public string HI105 { get; set; }
        public string HI106 { get; set; }
        public string HI107 { get; set; }
        public string HI108 { get; set; }
        public string HI109 { get; set; }
        public string HI110 { get; set; }
        public string HI111 { get; set; }
        public string HI112 { get; set; }
        public string HI113 { get; set; }
        public string HI114 { get; set; }
        public string HI115 { get; set; }
        public string HI116 { get; set; }
        public string HI117 { get; set; }
        public string HI118 { get; set; }
        public string HI119 { get; set; }
        public string HI120 { get; set; }
        public string HI121 { get; set; }
        public string HI122 { get; set; }
        public string HI123 { get; set; }
        public string HI124 { get; set; }
        public string HI125 { get; set; }
        public string HI126 { get; set; }
        public string HI127 { get; set; }
        public string HI128 { get; set; }
        public string HI129 { get; set; }
        public string HI130 { get; set; }
        public string HI131 { get; set; }
        public string HI132 { get; set; }
        public string HI133 { get; set; }
        public string HI134 { get; set; }
        public string HI135 { get; set; }
        public string HI136 { get; set; }
        public string HI137 { get; set; }
        public string HI138 { get; set; }
        public string HI139 { get; set; }
        public string HI140 { get; set; }
        public string HI141 { get; set; }
        public string HI142 { get; set; }
        public string HI143 { get; set; }
        public string HI144 { get; set; }
        public string HI145 { get; set; }
        public string HI146 { get; set; }
        public string HI147 { get; set; }
        public string HI148 { get; set; }
        public string HI149 { get; set; }
        public string HI150 { get; set; }
        public string HI151 { get; set; }
        public string HI152 { get; set; }
        public string HI153 { get; set; }
        public string HI154 { get; set; }
        public string HI155 { get; set; }
        public string HI156 { get; set; }
        public string HI157 { get; set; }
        public string HI158 { get; set; }
        public string HI159 { get; set; }
        public string HI160 { get; set; }
        public string HI161 { get; set; }
        public string HI162 { get; set; }
        public string HI163 { get; set; }
        public string HI164 { get; set; }
        public string HI165 { get; set; }
        public string HI166 { get; set; }
        public string HI167 { get; set; }
        public string HI168 { get; set; }
        public string HI169 { get; set; }
        public string HI170 { get; set; }
        public string HI171 { get; set; }
        public string HI172 { get; set; }
        public string HI173 { get; set; }
        public string HI174 { get; set; }
        public string HI175 { get; set; }
        public string HI176 { get; set; }
        public string HI177 { get; set; }
        public string HI178 { get; set; }
        public string HI179 { get; set; }
        public string HI180 { get; set; }
        public string HI181 { get; set; }
        public string HI182 { get; set; }
        public string HI183 { get; set; }
        public string HI184 { get; set; }
        public string HI185 { get; set; }
        public string HI186 { get; set; }
        public string HI187 { get; set; }
        public string HI188 { get; set; }
        public string HI189 { get; set; }
        public string HI190 { get; set; }
        public string HI191 { get; set; }
        public string HI192 { get; set; }
        public string HI193 { get; set; }
        public string HI194 { get; set; }
        public string HI195 { get; set; }
        public string HI196 { get; set; }
        public string HI197 { get; set; }
        public string HI198 { get; set; }
        public string HI199 { get; set; }
        public string HI200 { get; set; }
        public string HI201 { get; set; }
        public string HI202 { get; set; }
        public string HI203 { get; set; }
        public string HI204 { get; set; }
        public string HI205 { get; set; }
        public string HI206 { get; set; }
        public string HI207 { get; set; }
        public string HI208 { get; set; }
        public string HI209 { get; set; }
        public string HI210 { get; set; }
        public string HI211 { get; set; }
        public string HI212 { get; set; }
        public string HI213 { get; set; }
        public string HI214 { get; set; }
        public string HI215 { get; set; }
        public string HI216 { get; set; }
        public string HI217 { get; set; }
        public string HI218 { get; set; }
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

        public int HiProgramId { get; set; }
        public int? ProgramFolderId { get; set; }
        public int SurveyYear { get; set; }

        public DateTimeOffset? DateTimeSaved { get; set; }
        public DateTimeOffset? DateTimeSubmitted { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public DateTimeOffset? InsertedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; }
    }
}