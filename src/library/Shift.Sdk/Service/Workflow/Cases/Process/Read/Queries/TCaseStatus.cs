using System;

namespace InSite.Application.Cases.Read
{
    public class TCaseStatus
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid StatusIdentifier { get; set; }

        public string CaseType { get; set; }
        public string StatusCategory { get; set; }
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }

        /// <remarks>
        /// Currently this property is not referenced by part of the user interface, and it is not 
        /// used by any code within the platform. However, it is used by some customers (e.g., 
        /// Inspire in the E06 partition) in queries and reports that they build to summarize cases.
        /// Perhaps it should be renamed Reporting Category in a future version so its name is more
        /// self-documenting.
        /// </remarks>
        public string ReportCategory { get; set; }

        public int StatusSequence { get; set; }
    }
}
