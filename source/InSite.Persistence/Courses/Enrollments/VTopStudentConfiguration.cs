using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VTopStudentConfiguration : EntityTypeConfiguration<VTopStudent>
    {
        public VTopStudentConfiguration() : this("records") { }

        public VTopStudentConfiguration(string schema)
        {
            ToTable(schema + ".VTopStudent");
            HasKey(x => new { x.ProgressIdentifier });
        }
    }
}
