using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ActiveUser
    {
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String FullName { get; set; }
        public String LastName { get; set; }
        public String MiddleName { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset? UtcAuthenticated { get; set; }
    }
}
