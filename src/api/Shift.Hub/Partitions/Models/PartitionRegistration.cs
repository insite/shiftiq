namespace Shift.Hub.Partitions
{
    public class PartitionRegistration
    {
        public int Number { get; set; }
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Theme { get; set; } = null!;
        public string Domain { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public Guid Identifier { get; set; }
        public List<string> Whitelist { get; set; } = new();
        public string? HelpUrl { get; set; }
        public string? LogoUrl { get; set; }

        public List<OrganizationRegistration> Organizations { get; set; } = new();
    }
}
