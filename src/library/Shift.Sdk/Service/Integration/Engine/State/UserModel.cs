

namespace InSite.Domain.Integrations.Engine
{
    public class UserModel
    {
        
        public System.Guid UserIdentifier { get; set; }

        
        public string OrganizationCode { get; set; }

        
        public bool RoleIsRoot { get; set; }

        
        public string UserEmail { get; set; }

        
        public string UserName { get; set; }

        public System.Guid? TokenIdentifier { get; set; }
        public System.DateTimeOffset? TokenIssued { get; set; }
        public System.DateTimeOffset? TokenExpiry { get; set; }

        
        public string TokenSecret { get; set; }
    }
}
