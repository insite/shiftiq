using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UserRegistrationDetailConfiguration : EntityTypeConfiguration<UserRegistrationDetail>
    {
        public UserRegistrationDetailConfiguration() : this("contacts") { }

        public UserRegistrationDetailConfiguration(string schema)
        {
            ToTable(schema + ".UserRegistrationDetail");
            HasKey(x => new { x.UserIdentifier, x.RegistrationIdentifier });
        }
    }
}
