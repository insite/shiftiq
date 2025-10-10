namespace Shift.Api.Sdk.Core;

public static class ApiEndpoints
{
    private const string ApiBase = "/api";

    public static class Analytics
    {
        public static class Reports
        {
            public const string ComplianceSummaries = ApiBase + "/analytics/reports/compliance-summaries/export";
        }
    }
}