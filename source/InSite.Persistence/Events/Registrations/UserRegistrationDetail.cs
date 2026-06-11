using System;

namespace InSite.Persistence
{
    public class UserRegistrationDetail
    {
        public Guid UserIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string CompanyName { get; set; }
        public string OrganizationSnapshot { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string HomeAddressStreet1 { get; set; }
        public string HomeAddressStreet2 { get; set; }
        public string HomeAddressCity { get; set; }
        public string HomeAddressProvince { get; set; }
        public string HomeAddressPostalCode { get; set; }
        public string HomeAddressCountry { get; set; }
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressProvince { get; set; }
        public string ShippingAddressPostalCode { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string PersonCode { get; set; }
        public string Email { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementDescription { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public int? DurationQuantity { get; set; }
        public string DurationUnit { get; set; }
        public decimal? RegistrationFee { get; set; }
        public string ApprovalStatus { get; set; }
        public bool IncludeInT2202 { get; set; }
    }
}
