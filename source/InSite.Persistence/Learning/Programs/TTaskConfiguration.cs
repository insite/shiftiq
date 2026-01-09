using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TTaskConfiguration : EntityTypeConfiguration<TTask>
    {
        public TTaskConfiguration() : this("records") { }

        public TTaskConfiguration(string schema)
        {
            ToTable(schema + ".TTask");
            HasKey(x => new { x.TaskIdentifier });

            Property(x => x.ObjectIdentifier).IsRequired();
            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.TaskCompletionRequirement).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.TaskImage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.TaskIdentifier).IsRequired();
            Property(x => x.TaskIsRequired).IsRequired();
            Property(x => x.TaskLifetimeMonths).IsOptional();
            Property(x => x.TaskSequence).IsRequired();

            HasRequired(x => x.Program).WithMany(x => x.Tasks).HasForeignKey(x => x.ProgramIdentifier);
        }
    }
}
