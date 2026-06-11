namespace Shift.Hub.Partitions
{
    public class OrganizationRegistration
    {
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid Identifier { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? LogoUrl { get; set; }

        public AccountRegistration Account { get; set; } = new();
    }
}
