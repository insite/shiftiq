using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using InSite.Domain.Organizations;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Contacts.Reports.Models
{
    public class TaxFormHelper
    {
        private class TaxFormInfoItem
        {
            public class Registration
            {
                public int? FromYear { get; set; }
                public int? FromMonth { get; set; }
                public int? ToYear { get; set; }
                public int? ToMonth { get; set; }
                public decimal? TuitionFees { get; set; }
                public int? PartTimeMonths { get; set; }
                public int? FullTimeMonths { get; set; }
            }

            public readonly Registration[] Registrations = new Registration[4];

            public string T2202 => "T2202";
            public string CompanyName { get; set; }
            public string CompanyTag => "";
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public int TaxYear { get; set; }
            public string SIN { get; set; }
            public string SlipStatus => "O";
            public string ProgramName { get; set; }
            public object StudentNumber { get; set; }
            public string FlyingSchoolCourseType => "";

            public int? FromYear1 => Registrations[0]?.FromYear;
            public int? FromMonth1 => Registrations[0]?.FromMonth;
            public int? ToYear1 => Registrations[0]?.ToYear;
            public int? ToMonth1 => Registrations[0]?.ToMonth;
            public decimal? TuitionFees1 => Registrations[0]?.TuitionFees;
            public int? PartTimeMonths1 => Registrations[0]?.PartTimeMonths;
            public int? FullTimeMonths1 => Registrations[0]?.FullTimeMonths;

            public int? FromYear2 => Registrations[1]?.FromYear;
            public int? FromMonth2 => Registrations[1]?.FromMonth;
            public int? ToYear2 => Registrations[1]?.ToYear;
            public int? ToMonth2 => Registrations[1]?.ToMonth;
            public decimal? TuitionFees2 => Registrations[1]?.TuitionFees;
            public int? PartTimeMonths2 => Registrations[1]?.PartTimeMonths;
            public int? FullTimeMonths2 => Registrations[1]?.FullTimeMonths;

            public int? FromYear3 => Registrations[2]?.FromYear;
            public int? FromMonth3 => Registrations[2]?.FromMonth;
            public int? ToYear3 => Registrations[2]?.ToYear;
            public int? ToMonth3 => Registrations[2]?.ToMonth;
            public decimal? TuitionFees3 => Registrations[2]?.TuitionFees;
            public int? PartTimeMonths3 => Registrations[2]?.PartTimeMonths;
            public int? FullTimeMonths3 => Registrations[2]?.FullTimeMonths;

            public int? FromYear4 => Registrations[3]?.FromYear;
            public int? FromMonth4 => Registrations[3]?.FromMonth;
            public int? ToYear4 => Registrations[3]?.ToYear;
            public int? ToMonth4 => Registrations[3]?.ToMonth;
            public decimal? TuitionFees4 => Registrations[3]?.TuitionFees;
            public int? PartTimeMonths4 => Registrations[3]?.PartTimeMonths;
            public int? FullTimeMonths4 => Registrations[3]?.FullTimeMonths;

            public int? TextAtTop => (int?)null;
            public string Email { get; set; }
            public string OkToEmailSlip => "Yes";
            public string SlipTag => "";
            public string CustomField => $"{TaxYear} T2202";
            public string CustomPassword => "";

            public string CourseName { get; set; }
        }

        private static Regex RemoveNonDigit = new Regex("[^0-9]");
        private static Blowfish Blowfish => new Blowfish(EncryptionKey.Default);

        private const string IntFormat = "###############0";
        private const string NumberFormat = "###,###,###,##0.00";

        public static byte[] GetTaxFormInfoXlsx(int year)
        {
            var data = GetData(year);
            if (data.Count == 0)
                return null;

            var helper = new XlsxExportHelper();

            helper.Map("T2202", "T2202", 7, HorizontalAlignment.Left);
            helper.Map("CompanyName", "Company.Name1", 35, HorizontalAlignment.Left);
            helper.Map("CompanyTag", "Company.CompanyTag", 25, HorizontalAlignment.Left);
            helper.Map("LastName", "LastName", 15, HorizontalAlignment.Left);
            helper.Map("FirstName", "FirstName", 15, HorizontalAlignment.Left);
            helper.Map("MiddleName", "Initial", 10, HorizontalAlignment.Left);
            helper.Map("Street1", "Address1", 25, HorizontalAlignment.Left);
            helper.Map("Street2", "Address2", 25, HorizontalAlignment.Left);
            helper.Map("City", "City", 15, HorizontalAlignment.Left);
            helper.Map("Province", "Prov", 10, HorizontalAlignment.Left);
            helper.Map("PostalCode", "Postal", 15, HorizontalAlignment.Left);
            helper.Map("Country", "Country", 15, HorizontalAlignment.Left);
            helper.Map("TaxYear", "TaxYear", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("SIN", "SIN", 15, HorizontalAlignment.Left);
            helper.Map("SlipStatus", "SlipStatus", 15, HorizontalAlignment.Left);
            helper.Map("ProgramName", "ProgramName", 30, HorizontalAlignment.Left);
            helper.Map("StudentNumber", "StudentNumber", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FlyingSchoolCourseType", "FlyingSchoolCourseType", 25, HorizontalAlignment.Left);
            helper.Map("FromYear1", "FromYear1", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("FromMonth1", "FromMonth1", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("ToYear1", "ToYear1", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("ToMonth1", "ToMonth1", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("TuitionFees1", "TuitionFees1", NumberFormat, 15, HorizontalAlignment.Right);
            helper.Map("PartTimeMonths1", "PartTimeMonths1", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FullTimeMonths1", "FullTimeMonths1", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FromYear2", "FromYear2", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("FromMonth2", "FromMonth2", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("ToYear2", "ToYear2", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("ToMonth2", "ToMonth2", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("TuitionFees2", "TuitionFees2", NumberFormat, 15, HorizontalAlignment.Right);
            helper.Map("PartTimeMonths2", "PartTimeMonths2", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FullTimeMonths2", "FullTimeMonths2", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FromYear3", "FromYear3", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("FromMonth3", "FromMonth3", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("ToYear3", "ToYear3", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("ToMonth3", "ToMonth3", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("TuitionFees3", "TuitionFees3", NumberFormat, 15, HorizontalAlignment.Right);
            helper.Map("PartTimeMonths3", "PartTimeMonths3", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FullTimeMonths3", "FullTimeMonths3", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FromYear4", "FromYear4", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("FromMonth4", "FromMonth4", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("ToYear4", "ToYear4", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("ToMonth4", "ToMonth4", IntFormat, 10, HorizontalAlignment.Right);
            helper.Map("TuitionFees4", "TuitionFees4", NumberFormat, 15, HorizontalAlignment.Right);
            helper.Map("PartTimeMonths4", "PartTimeMonths4", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("FullTimeMonths4", "FullTimeMonths4", IntFormat, 20, HorizontalAlignment.Right);
            helper.Map("TextAtTop", "TextAtTop", IntFormat, 15, HorizontalAlignment.Right);
            helper.Map("Email", "EmailAddress", 30, HorizontalAlignment.Left);
            helper.Map("OkToEmailSlip", "OkToEmailSlip", 15, HorizontalAlignment.Left);
            helper.Map("SlipTag", "SlipTag", 15, HorizontalAlignment.Left);
            helper.Map("CustomField", "CustomField", 15, HorizontalAlignment.Left);
            helper.Map("CustomPassword", "CustomPassword", 20, HorizontalAlignment.Left);
            helper.Map("CourseName", "Course Name", 30, HorizontalAlignment.Left);

            return helper.GetXlsxBytes(data, "Tax Form T2202 Info");
        }

        private static List<TaxFormInfoItem> GetData(int year)
        {
            var rows = UserSearch.SelectUserT2202Detail(CurrentSessionState.Identity.Organization.OrganizationIdentifier, year);
            var result = new List<TaxFormInfoItem>();
            var dict = new Dictionary<Tuple<Guid, string>, TaxFormInfoItem>();
            OrganizationState organization = null;

            foreach (var row in rows)
            {
                var key = new Tuple<Guid, string>(row.UserIdentifier, row.AchievementDescription ?? Guid.NewGuid().ToString());

                if (!dict.TryGetValue(key, out var item))
                {
                    if (organization == null)
                        organization = JsonConvert.DeserializeObject<OrganizationState>(row.OrganizationSnapshot);

                    var rawSIN = Blowfish.DecipherString(row.SocialInsuranceNumber);
                    var sin = !string.IsNullOrEmpty(rawSIN) ? RemoveNonDigit.Replace(rawSIN, "") : null;

                    item = new TaxFormInfoItem
                    {
                        CompanyName = organization.CompanyName,
                        LastName = row.LastName,
                        FirstName = row.FirstName,
                        MiddleName = !string.IsNullOrEmpty(row.MiddleName) ? row.MiddleName.Substring(0, 1) : null,
                        SIN = sin,
                        StudentNumber = int.TryParse(row.PersonCode, out var personCode) ? personCode : (object)row.PersonCode,
                        Email = row.Email,
                        TaxYear = row.EventScheduledStart.Year,
                        ProgramName = row.AchievementDescription,
                        CourseName = row.EventTitle,
                        Country = "CAN"
                    };

                    if (!string.IsNullOrEmpty(row.HomeAddressStreet1)
                        || !string.IsNullOrEmpty(row.HomeAddressStreet2)
                        || !string.IsNullOrEmpty(row.HomeAddressCity)
                        || !string.IsNullOrEmpty(row.HomeAddressProvince)
                        || !string.IsNullOrEmpty(row.HomeAddressPostalCode)
                        )
                    {
                        item.Street1 = row.HomeAddressStreet1;
                        item.Street2 = row.HomeAddressStreet2;
                        item.City = row.HomeAddressCity;
                        item.Province = row.HomeAddressProvince;
                        item.PostalCode = row.HomeAddressPostalCode;
                    }
                    else
                    {
                        item.Street1 = row.ShippingAddressStreet1;
                        item.Street2 = row.ShippingAddressStreet2;
                        item.City = row.ShippingAddressCity;
                        item.Province = row.ShippingAddressProvince;
                        item.PostalCode = row.ShippingAddressPostalCode;
                    }

                    result.Add(item);
                    dict.Add(key, item);
                }

                AddRegistration(item, row);
            }

            return result;
        }

        private static void AddRegistration(TaxFormInfoItem item, UserRegistrationDetail row)
        {
            TaxFormInfoItem.Registration registration = null;

            for (int i = 0; i < item.Registrations.Length; i++)
            {
                if (item.Registrations[i] == null)
                {
                    item.Registrations[i] = registration = new TaxFormInfoItem.Registration();

                    if (i > 0)
                    {
                        item.CourseName = !string.IsNullOrEmpty(item.CourseName)
                            ? item.CourseName + "; " + row.EventTitle
                            : row.EventTitle;
                    }

                    break;
                }
            }

            if (registration == null)
                throw new ArgumentOutOfRangeException($"User key = {row.UserIdentifier} has more registrations for '{row.AchievementDescription}' than {item.Registrations.Length}");

            registration.FromYear = row.EventScheduledStart.Year;
            registration.FromMonth = row.EventScheduledStart.Month;
            registration.ToYear = row.EventScheduledEnd != null ? row.EventScheduledEnd.Value.Year : (int?)null;
            registration.ToMonth = row.EventScheduledEnd != null ? row.EventScheduledEnd.Value.Month : (int?)null;
            registration.TuitionFees = row.RegistrationFee;
            registration.PartTimeMonths = 0;
            registration.FullTimeMonths = 0;

            if (string.Equals(row.DurationUnit, "Week", StringComparison.OrdinalIgnoreCase) && row.DurationQuantity.HasValue)
            {
                if (row.DurationQuantity == 6)
                    registration.FullTimeMonths = 2;
                else if (row.DurationQuantity == 4)
                    registration.FullTimeMonths = 1;
            }
        }
    }
}