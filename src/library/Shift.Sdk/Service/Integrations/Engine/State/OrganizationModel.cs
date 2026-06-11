namespace InSite.Domain.Integrations.Engine
{
    public class OrganizationModel
    {
        public string Code { get; set; } 
        public string Domain { get; set; } 
        public System.Guid Identifier { get; set; } 
        public string Name { get; set; } 

        public EnterpriseModel Enterprise { get; set; } 
        public int Number { get; set; }
    }
}
