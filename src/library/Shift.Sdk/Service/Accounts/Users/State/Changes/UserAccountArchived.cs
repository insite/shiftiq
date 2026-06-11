using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Users
{
    public class UserAccountArchived : Change
    {
        public Guid TenantIdentifier { get; set; }
        public string PersonName { get; set; }
        public string PersonEmail { get; set; }
        public string Status { get; set; }

        public UserAccountArchived(Guid tenantID, string personName, string personEmail, string status)
        {
            TenantIdentifier = tenantID;
            PersonName = personName;
            PersonEmail = personEmail;
            Status = status;
        }
    }
}