using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardValidationChangeConfiguration : EntityTypeConfiguration<StandardValidationChange>
    {
        public StandardValidationChangeConfiguration() : this("standards") { }

        public StandardValidationChangeConfiguration(string schema)
        {
            ToTable(schema + ".StandardValidationChange");
            HasKey(x => new { x.ChangeIdentifier });

            Property(x => x.ChangeComment).IsOptional().IsUnicode(false);
            Property(x => x.ChangeStatus).IsRequired().IsUnicode(false).HasMaxLength(50);

            HasRequired(x => x.Standard).WithMany(x => x.StandardValidationChanges).HasForeignKey(x => x.StandardIdentifier);
            HasRequired(x => x.User).WithMany(x => x.UserStandardValidationChanges).HasForeignKey(x => x.UserIdentifier);
            HasOptional(x => x.Author).WithMany(x => x.AuthorStandardValidationChanges).HasForeignKey(x => x.AuthorUserIdentifier);
        }
    }
}
