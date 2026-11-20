namespace InSite.Application.Records.Read
{
    // The string values below are the RequestStatus and RequestFundingPath domains. They match the
    // CHECK constraints on records.TProgramEnrollmentRequest exactly - change them together.

    public static class ProgramEnrollmentRequestStatus
    {
        public const string Requested = "requested";
        public const string Approved = "approved";
        public const string Rejected = "rejected";
        public const string Superseded = "superseded";
    }

    public static class ProgramEnrollmentRequestFundingPath
    {
        public const string SelfFunded = "self-funded";
        public const string CompanySponsored = "company-sponsored";
    }
}
