using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Users
{
    public class UserAccountWelcomed : Change
    {
        public string UserFirstName{ get; set; }
        public string UserEmail{ get; set; }
        public string UserPassword{ get; set; }
        public string UserPasswordHash{ get; set; }

        public string TenantCode { get; set; }
        public Guid TenantIdentifier { get; set; }
        public string TenantName{ get; set; }
        
        public DateTimeOffset? UserAccessGranted{ get; set; }
        public Guid? UserIdentifier { get; set; }
    }
}