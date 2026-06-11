namespace InSite.Domain.Integrations.Engine
{
    public class UserClaimModel
    {
        public System.Guid Identifier { get; set; } 
        public string Type { get; set; } 
        public string Value { get; set; } 

        public System.Guid User { get; set; } 
    }
}
