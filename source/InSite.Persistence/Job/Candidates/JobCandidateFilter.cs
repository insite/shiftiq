using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class JobCandidateFilter : Filter
    {
        public string[] Cities { get; set; }
        public bool IsActivelySeeking { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsHaveEmptyName { get; set; }

        // Minimum language level
        public int MinSpokenEnglishLevel { get; set; }
        public int MinListeningEnglishLevel { get; set; }
        public int MinWrittenEnglishLevel { get; set; }
        public int MinReadingEnglishLevel { get; set; }

        // Qualification
        public bool HightSchoolDiploma { get; set; }
        public bool CollegeUniversityCertificate { get; set; }
        public bool CollegeDiploma { get; set; }
        public bool TradesCertificate { get; set; }
        public bool BachelorsDegree { get; set; }
        public bool MastersDegree { get; set; }
        public bool DoctoralDegree { get; set; }

        // Immigration Status
        public bool LandedRefugee { get; set; }
        public bool ApprovedImmigrationInCanada { get; set; }
        public bool LandedImmigrantInCanada { get; set; }
        public bool UkrainianImmigrantOpenWorkPermit { get; set; }

        public string Occupation { get; set; }

        public string Keywords { get; set; }
    }
}
