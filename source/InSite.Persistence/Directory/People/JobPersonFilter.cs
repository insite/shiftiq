﻿using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class JobPersonFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? AreaOfInterest { get; set; }

        public string[] Cities { get; set; }

        public string Occupation { get; set; }
        public string Keywords { get; set; }
        public string Qualification { get; set; }
        public string CurrentJobTitle { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }

        public bool? WillingToRelocate { get; set; }
        public bool? IsApproved { get; set; }

        public bool IsActivelySeeking { get; set; }
        public bool HightSchoolDiploma { get; set; }
        public bool CollegeUniversityCertificate { get; set; }
        public bool CollegeDiploma { get; set; }
        public bool TradesCertificate { get; set; }
        public bool BachelorsDegree { get; set; }
        public bool MastersDegree { get; set; }
        public bool DoctoralDegree { get; set; }
        public bool LandedRefugee { get; set; }
        public bool ApprovedImmigrationInCanada { get; set; }
        public bool LandedImmigrantInCanada { get; set; }
        public bool UkrainianImmigrantOpenWorkPermit { get; set; }

    }
}
