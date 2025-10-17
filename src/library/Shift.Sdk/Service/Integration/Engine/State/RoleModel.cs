

namespace InSite.Domain.Integrations.Engine
{
    public class RoleModel
    {
        public System.Guid Identifier { get; set; } 
        public string Name { get; set; } 

        public System.Guid Organization { get; set; }

        public Timestamp Modified { get; set; }
    }
}
