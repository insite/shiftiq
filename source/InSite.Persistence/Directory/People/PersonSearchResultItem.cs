using System;

namespace InSite.Persistence
{
    public class PersonSearchResultItem
    {
        public Guid UserIdentifier { get; internal set; }
        public string FirstName { get; internal set; }
        public string FullName { get; internal set; }
        public string Email { get; internal set; }
        public string EmailAlternate { get; internal set; }
        public string LastName { get; internal set; }
        public Guid ModifiedBy { get; internal set; }
        public string Phone { get; internal set; }
        public string PhoneHome { get; internal set; }
        public DateTime? Birthdate { get; internal set; }
        public Address ShippingAddress { get; internal set; }
        public Address WorkAddress { get; internal set; }
        public Address HomeAddress { get; internal set; }
        public DateTimeOffset Created { get; internal set; }
        public DateTimeOffset Modified { get; internal set; }
        public string StatusHtmlColor { get; internal set; }
        public string StatusText { get; internal set; }
        public int ResourceCount { get; internal set; }
        public int StandardCount { get; internal set; }
        public int GroupCount { get; internal set; }
        public string OrganizationName { get; internal set; }

        public Guid? EmployerGroupIdentifier { get; internal set; }
        public string EmployerGroupCode { get; internal set; }
        public string EmployerGroupName { get; internal set; }
        public string EmployerGroupRegion { get; internal set; }

        public Guid? EmployerDistrictIdentifier { get; internal set; }
        public string EmployerDistrictAccountNumber { get; internal set; }
        public string EmployerDistrictName { get; internal set; }
        public Guid OrganizationIdentifier { get; internal set; }
        public string UserAccessGrantedBy { get; internal set; }
        public bool IsArchived { get; internal set; }
        public bool IsApproved { get; internal set; }
        public string UserPasswordHash { get; internal set; }
        public string Honorific { get; internal set; }
        public bool EmailEnabled { get; internal set; }
        public string JobTitle { get; internal set; }
        public string ImageUrl { get; internal set; }
        public DateTimeOffset? UserAccessGranted { get; internal set; }
        public int SessionCount { get; internal set; }
        public DateTimeOffset? LastAuthenticated { get; set; }
        public string PermissionLists { get; set; }
        public string PersonCode { get; set; }
        public string Region { get; set; }
    }
}
