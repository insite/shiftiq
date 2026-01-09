namespace InSite.Domain.Integrations.Engine
{
    public class RoleClaimModel
    {
        public System.Guid Identifier { get; set; } 
        public string Type { get; set; } 
        public string Value { get; set; } 

        public System.Guid Role { get; set; } 
    }
}
