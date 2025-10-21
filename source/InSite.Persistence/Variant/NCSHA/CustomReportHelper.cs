using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class CustomReportHelper
    {
        #region Classes

        public class ReportSession
        {
            public string SessionId { get; set; }
            public DateTimeOffset Expires { get; set; }

            public static ReportSession Decode(string encoded)
            {
                var data = Convert.FromBase64String(encoded);
                var json = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<ReportSession>(json);
            }

            public static string Encode(ReportSession session)
            {
                var json = JsonConvert.SerializeObject(session);
                var data = Encoding.UTF8.GetBytes(json);
                return Convert.ToBase64String(data);
            }
        }

        [Serializable]
        public class Report
        {
            public string Code { get; private set; }
            public string CriteriaPath { get; private set; }
            public string Title { get; private set; }
            public string Package { get; private set; }
            public string NavigateUrl { get; set; }

            public string GetPreviewUrl()
            {
                var query = $"/ui/portal/plugin/ncsha/report?report={Code}";
                return query;
            }

            public Report(string code, string criteriaPath, string title, string package = null)
            {
                Code = code;
                CriteriaPath = criteriaPath;
                Title = title;
                Package = package;
            }
        }

        #endregion

        #region Fields

        private static List<Report> _reports;

        #endregion

        #region Construction

        static CustomReportHelper()
        {
            _reports = new List<Report>
            {
                new Report("AB-Landscape", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "AB Report Bundle (landscape)", "NCSHA: Administration and Budget"),
                new Report("AB-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "AB Report Bundle (portrait)", "NCSHA: Administration and Budget"),
                new Report("AB01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Agency Profile", "NCSHA: Administration and Budget"),
                new Report("AB02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Board of Directors Profile", "NCSHA: Administration and Budget"),
                new Report("AB03", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Executive Director Profile", "NCSHA: Administration and Budget"),
                new Report("AB04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Organization Staffing and Budget", "NCSHA: Administration and Budget"),
                new Report("AB05", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Bonds Outstanding", "NCSHA: Administration and Budget"),
                new Report("AB06", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Agency Reserves", "NCSHA: Administration and Budget"),
                new Report("AB07", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Consolidated Plan", "NCSHA: Administration and Budget"),
                new Report("AB08", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Single-Family Programs in Operation", "NCSHA: Administration and Budget"),
                new Report("AB09", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Multifamily Programs in Operation", "NCSHA: Administration and Budget"),
                new Report("AB10", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Special Needs Housing Programs in Operation", "NCSHA: Administration and Budget"),
                new Report("AB11", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Other HFA Programs in Operation", "NCSHA: Administration and Budget"),
                new Report("AB12", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "State Funding for Housing", "NCSHA: Administration and Budget"),
                new Report("AB13a", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Federal Program Utilization A", "NCSHA: Administration and Budget"),
                new Report("AB13b", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Federal Program Utilization B", "NCSHA: Administration and Budget"),
                new Report("AB14", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Federal Home Loan Bank Partnerships", "NCSHA: Administration and Budget"),
                new Report("AB15", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Federal Home Loan Bank Programs", "NCSHA: Administration and Budget"),
                new Report("HC-Landscape", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HC Report Bundle (landscape)", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HC Report Bundle (portrait)", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Housing Credit Authority", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Housing Credit Applications and Allocations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC03a", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Units Receiving Allocations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC03b", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Low Income Housing Tax Credit", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC03c", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Low Income Housing Tax Credit", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Tax-Exempt Bond Allocations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC05", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Tax-Exempt Bond Units Receiving Allocations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC06", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Resyndication – Credit Units and Dollars Allocated", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC07", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Housing Credit Production by Unit Size", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC08", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of Housing Credit Units Receiving Other Federal Subsidies", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC09", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Minimum Set-Aside and Basis Boosts", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC10a", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Targeting of 9 Percent Housing Credit Units"),
                new Report("HC10b", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Targeting of 4 Percent/Bond Housing Credit Units"),
                new Report("HC11", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of Housing Credit Units for Special Needs Populations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC12", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Nonprofit Allocations", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC13", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Set-Asides", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC14", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Extended Low-Income Use Restrictions", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC15", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Compliance Monitoring and Asset Management", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC16", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Fee Structure", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC17", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "State Tax Credits", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC18", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Preservation", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC19", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "State-Determined Basis Boost", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HC20", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Housing Credit Authority and Units in RAD Program Properties", "NCSHA: Low Income Housing Tax Credits"),
                new Report("HI-Landscape", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HI Report Bundle (landscape)", "NCSHA: HOME Investment Partnerships"),
                new Report("HI-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HI Report Bundle (portrait)", "NCSHA: HOME Investment Partnerships"),
                new Report("HI01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "1:Total HOME Funds Allocated Statewide", "NCSHA: HOME Investment Partnerships"),
                new Report("HI02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "2a:HFA-Administered HOME Funds", "NCSHA: HOME Investment Partnerships"),
                new Report("HI03", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HOME Funds Committed - Single-Family (continued)", "NCSHA: HOME Investment Partnerships"),
                new Report("HI04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HOME Funds Committed - Multifamily and Organization-Based Rental Assistance", "NCSHA: HOME Investment Partnerships"),
                new Report("HI05", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Total HOME Funds Committed", "NCSHA: HOME Investment Partnerships"),
                new Report("HI06", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Total HOME Funds Requested", "NCSHA: HOME Investment Partnerships"),
                new Report("HI07", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HOME Funds for Specific Populations", "NCSHA: HOME Investment Partnerships"),
                new Report("HI08", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HOME Units for Specific Populations", "NCSHA: HOME Investment Partnerships"),
                new Report("HI09", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of HOME Assisted Units Targeted by Income", "NCSHA: HOME Investment Partnerships"),
                new Report("HI10", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HOME Funds Committed to CHDOs and Non-CHDO Nonprofits", "NCSHA: HOME Investment Partnerships"),
                new Report("HI11", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of HOME Units Receiving Other Federal Subsidies", "NCSHA: HOME Investment Partnerships"),
                new Report("HI12", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Sources of HOME Match Funds", "NCSHA: HOME Investment Partnerships"),
                new Report("MF-Landscape", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MF Report Bundle (landscape)", "NCSHA: Multifamily Bonds"),
                new Report("MF-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MF Report Bundle (portrait)", "NCSHA: Multifamily Bonds"),
                new Report("MF01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Number of Multifamily Bond Issues", "NCSHA: Multifamily Bonds"),
                new Report("MF02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Dollar Amount of Multifamily Bond Issues", "NCSHA: Multifamily Bonds"),
                new Report("MF03", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Expected Units From Multifamily Bond Issues", "NCSHA: Multifamily Bonds"),
                new Report("MF04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Multifamily Bonds Properties and Units Granted a Certificate of Occupancy or Placed in Service in Current Year from Bonds Issued in any Year", "NCSHA: Multifamily Bonds"),
                new Report("MF05", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Income Targeting and Size of Multifamily Bonds Units Granted a Certificate of Occupancy or Placed in Service", "NCSHA: Multifamily Bonds"),
                new Report("MF06", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of Multifamily Bond Units Receiving Other Federal Subsidies", "NCSHA: Multifamily Bonds"),
                new Report("MF07", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Multifamily Bond Units for Special Needs Populations", "NCSHA: Multifamily Bonds"),
                new Report("MF08", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Bond Issuance Utilizing Credit Enhancement/Insurance", "NCSHA: Multifamily Bonds"),
                new Report("MF09", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Risk-Sharing", "NCSHA: Multifamily Bonds"),
                new Report("MF10", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Other Multifamily Production (Excluding Bonds, Housing Credits, or HOME)", "NCSHA: Multifamily Bonds"),
                new Report("MF11", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Properties and Units in Current Multifamily Portfolio", "NCSHA: Multifamily Bonds"),
                new Report("MF12", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Number of Properties in Multifamily Portfolio by Type", "NCSHA: Multifamily Bonds"),
                new Report("MF13", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Number of Units in Multifamily Portfolio by Type", "NCSHA: Multifamily Bonds"),
                new Report("MR-Landscape", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MR Report Bundle (landscape)", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MR Report Bundle (portrait)", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MRB Production", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MRB Mortgage and Borrower", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR03", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MRB Mortgage Distribution by Income and Area", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of MRB Loans Receiving Mortgage Insurance", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR05", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MCC Production", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR06", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MCC Mortgage and Borrower Characteristics", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR07", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "MCC Mortgage Distribution by Income and Area", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR08", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Other Single-Family Production", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR09", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Single-Family Mortgage and Borrower Characteristics", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR10", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Mortgage Distribution by Income Area", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR11", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Percent of HFA Single-Family Loans Receiving Mortgage Insurance", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR12", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Targeting of HFA Single-Family Funds to Special Groups", "NCSHA: Mortgage Revenue Bonds"),
                new Report("MR13", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Single-Family Portfolio Servicing", "NCSHA: Mortgage Revenue Bonds"),
                new Report("PA-Portrait", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "PA Report Bundle (portrait)", "NCSHA: Private Activity Bonds"),
                new Report("PA01", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "State Cap Authority (per capita)", "NCSHA: Private Activity Bonds"),
                new Report("PA02", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Authority and Usage", "NCSHA: Private Activity Bonds"),
                new Report("PA03", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "HFA Cap Issuance", "NCSHA: Private Activity Bonds"),
                new Report("PA04", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "State Allocation of Bond Cap Among Issuers", "NCSHA: Private Activity Bonds"),
                new Report("MR14", "/UI/Variant/NCSHA/Analytics/Controls/ReportCriteriaYear.ascx", "Single-Family Loans with Down Payment Assistance", "NCSHA: Low Income Housing Tax Credits")
            };
        }

        #endregion

        #region Methods

        public static Report GetReport(string code)
        {
            return _reports.Find(x => x.Code == code);
        }

        public static IEnumerable<Report> FindReports(string title)
        {
            return _reports.Where(x => x.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static IEnumerable<string> GetTitles()
        {
            return _reports.Select(x => x.Title).Distinct().OrderBy(x => x);
        }

        public static IEnumerable<Report> GetReports()
        {
            return _reports.OrderBy(x => x.Title);
        }

        public static bool GetReportPath(string reportNumber, string organizationCode, out string path)
        {
            path = null;

            if (reportNumber.StartsWith("/"))
                reportNumber = reportNumber.Substring(1);

            var match = Regex.Match(reportNumber, @"^R\d\d-(Cape|Ncsha)$");

            if (match.Success)
                path = $"/{match.Groups[1].Value}/{reportNumber}";
            else if (organizationCode == "ncsha")
                path = $"/Ncsha/{reportNumber}";
            else
                return false;

            return true;
        }

        #endregion
    }
}