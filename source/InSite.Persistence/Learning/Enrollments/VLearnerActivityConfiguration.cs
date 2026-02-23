using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VLearnerActivityConfiguration : EntityTypeConfiguration<VLearnerActivity>
    {
        public VLearnerActivityConfiguration() : this("records") { }

        public VLearnerActivityConfiguration(string schema)
        {
            ToTable(schema + ".VLearnerActivity");
            HasKey(x => x.FakeKey);
        }
    }
}
