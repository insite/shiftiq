using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI
{
    public static class NcshaReportHelper
    {
        private static readonly List<NcshaReport> _reports;

        static NcshaReportHelper()
        {
            _reports = new List<NcshaReport>
            {
                new NcshaReport("AB-Landscape", "AB NcshaReport Bundle (landscape)"),
                new NcshaReport("AB-Portrait", "AB NcshaReport Bundle (portrait)"),
                new NcshaReport("AB01", "Agency Profile"),
                new NcshaReport("AB02", "Board of Directors Profile"),
                new NcshaReport("AB03", "Executive Director Profile"),
                new NcshaReport("AB04", "Organization Staffing and Budget"),
                new NcshaReport("AB05", "Bonds Outstanding"),
                new NcshaReport("AB06", "Agency Reserves"),
                new NcshaReport("AB07", "Consolidated Plan"),
                new NcshaReport("AB08", "HFA Single-Family Programs in Operation"),
                new NcshaReport("AB09", "HFA Multifamily Programs in Operation"),
                new NcshaReport("AB10", "HFA Special Needs Housing Programs in Operation"),
                new NcshaReport("AB11", "Other HFA Programs in Operation"),
                new NcshaReport("AB12", "State Funding for Housing"),
                new NcshaReport("AB13a", "Federal Program Utilization A"),
                new NcshaReport("AB13b", "Federal Program Utilization B"),
                new NcshaReport("AB14", "Federal Home Loan Bank Partnerships"),
                new NcshaReport("AB15", "Federal Home Loan Bank Programs"),

                new NcshaReport("HC-Landscape", "HC NcshaReport Bundle (landscape)"),
                new NcshaReport("HC-Portrait", "HC NcshaReport Bundle (portrait)"),
                new NcshaReport("HC01", "Housing Credit Authority"),
                new NcshaReport("HC02", "Housing Credit Applications and Allocations"),
                new NcshaReport("HC03a", "Units Receiving Allocations"),
                new NcshaReport("HC03b", "Low Income Housing Tax Credit"),
                new NcshaReport("HC03c", "Low Income Housing Tax Credit"),
                new NcshaReport("HC04", "Tax-Exempt Bond Allocations"),
                new NcshaReport("HC05", "Tax-Exempt Bond Units Receiving Allocations"),
                new NcshaReport("HC06", "Resyndication – Credit Units and Dollars Allocated"),
                new NcshaReport("HC07", "Housing Credit Production by Unit Size"),
                new NcshaReport("HC08", "Percent of Housing Credit Units Receiving Other Federal Subsidies"),
                new NcshaReport("HC09", "Minimum Set-Aside and Basis Boosts"),
                new NcshaReport("HC10a", "Targeting of 9 Percent Housing Credit Units"),
                new NcshaReport("HC10b", "Targeting of 4 Percent/Bond Housing Credit Units"),
                new NcshaReport("HC11", "Percent of Housing Credit Units for Special Needs Populations"),
                new NcshaReport("HC12", "Nonprofit Allocations"),
                new NcshaReport("HC13", "Set-Asides"),
                new NcshaReport("HC14", "Extended Low-Income Use Restrictions"),
                new NcshaReport("HC15", "Compliance Monitoring and Asset Management"),
                new NcshaReport("HC16", "Fee Structure"),
                new NcshaReport("HC17", "State Tax Credits"),
                new NcshaReport("HC18", "Preservation"),
                new NcshaReport("HC19", "State-Determined Basis Boost"),
                new NcshaReport("HC20", "Housing Credit Authority and Units in RAD Program Properties"),

                new NcshaReport("HI-Landscape", "HI NcshaReport Bundle (landscape)"),
                new NcshaReport("HI-Portrait", "HI NcshaReport Bundle (portrait)"),
                new NcshaReport("HI01", "1:Total HOME Funds Allocated Statewide"),
                new NcshaReport("HI02", "2a:HFA-Administered HOME Funds"),
                new NcshaReport("HI03", "HOME Funds Committed - Single-Family (continued)"),
                new NcshaReport("HI04", "HOME Funds Committed - Multifamily and Tenant-Based Rental Assistance"),
                new NcshaReport("HI05", "Total HOME Funds Committed"),
                new NcshaReport("HI06", "Total HOME Funds Requested"),
                new NcshaReport("HI07", "HOME Funds for Specific Populations"),
                new NcshaReport("HI08", "HOME Units for Specific Populations"),
                new NcshaReport("HI09", "Percent of HOME Assisted Units Targeted by Income"),
                new NcshaReport("HI10", "HOME Funds Committed to CHDOs and Non-CHDO Nonprofits"),
                new NcshaReport("HI11", "Percent of HOME Units Receiving Other Federal Subsidies"),
                new NcshaReport("HI12", "Sources of HOME Match Funds"),

                new NcshaReport("MF-Landscape", "MF NcshaReport Bundle (landscape)"),
                new NcshaReport("MF-Portrait", "MF NcshaReport Bundle (portrait)"),
                new NcshaReport("MF01", "Number of Multifamily Bond Issues"),
                new NcshaReport("MF02", "Dollar Amount of Multifamily Bond Issues"),
                new NcshaReport("MF03", "Expected Units From Multifamily Bond Issues"),
                new NcshaReport("MF04", "Multifamily Bonds Properties and Units Granted a Certificate of Occupancy or Placed in Service in Current Year from Bonds Issued in any Year"),
                new NcshaReport("MF05", "Income Targeting and Size of Multifamily Bonds Units Granted a Certificate of Occupancy or Placed in Service"),
                new NcshaReport("MF06", "Percent of Multifamily Bond Units Receiving Other Federal Subsidies"),
                new NcshaReport("MF07", "Multifamily Bond Units for Special Needs Populations"),
                new NcshaReport("MF08", "Bond Issuance Utilizing Credit Enhancement/Insurance"),
                new NcshaReport("MF09", "Risk-Sharing"),
                new NcshaReport("MF10", "Other Multifamily Production (Excluding Bonds, Housing Credits, or HOME)"),
                new NcshaReport("MF11", "Properties and Units in Current Multifamily Portfolio"),
                new NcshaReport("MF12", "Number of Properties in Multifamily Portfolio by Type"),
                new NcshaReport("MF13", "Number of Units in Multifamily Portfolio by Type"),

                new NcshaReport("MR-Landscape", "MR NcshaReport Bundle (landscape)"),
                new NcshaReport("MR-Portrait", "MR NcshaReport Bundle (portrait)"),
                new NcshaReport("MR01", "MRB Production"),
                new NcshaReport("MR02", "MRB Mortgage and Borrower"),
                new NcshaReport("MR03", "MRB Mortgage Distribution by Income and Area"),
                new NcshaReport("MR04", "Percent of MRB Loans Receiving Mortgage Insurance"),
                new NcshaReport("MR05", "MCC Production"),
                new NcshaReport("MR06", "MCC Mortgage and Borrower Characteristics"),
                new NcshaReport("MR07", "MCC Mortgage Distribution by Income and Area"),
                new NcshaReport("MR08", "Other Single-Family Production"),
                new NcshaReport("MR09", "HFA Single-Family Mortgage and Borrower Characteristics"),
                new NcshaReport("MR10", "HFA Mortgage Distribution by Income Area"),
                new NcshaReport("MR11", "Percent of HFA Single-Family Loans Receiving Mortgage Insurance"),
                new NcshaReport("MR12", "Targeting of HFA Single-Family Funds to Special Groups"),
                new NcshaReport("MR13", "Single-Family Portfolio Servicing"),
                new NcshaReport("MR14", "Single-Family Loans with Down Payment Assistance"),

                new NcshaReport("PA-Portrait", "PA NcshaReport Bundle (portrait)"),
                new NcshaReport("PA01", "State Cap Authority (per capita)"),
                new NcshaReport("PA02", "HFA Authority and Usage"),
                new NcshaReport("PA03", "HFA Cap Issuance"),
                new NcshaReport("PA04", "State Allocation of Bond Cap Among Issuers")
            };
        }

        public static NcshaReport GetReport(string code)
        {
            return _reports.Find(x => x.Code == code);
        }

        public static IEnumerable<NcshaReport> FindReports(string title)
        {
            return _reports.Where(x => x.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static IEnumerable<string> GetTitles()
        {
            return _reports.Select(x => x.Title).Distinct().OrderBy(x => x);
        }

        public static IEnumerable<NcshaReport> GetReports()
        {
            return _reports.OrderBy(x => x.Code);
        }
    }
}