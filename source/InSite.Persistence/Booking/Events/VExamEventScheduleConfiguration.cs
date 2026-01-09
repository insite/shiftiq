using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VExamEventScheduleConfiguration : EntityTypeConfiguration<VExamEventSchedule>
    {
        public VExamEventScheduleConfiguration() : this("reports") { }

        public VExamEventScheduleConfiguration(string schema)
        {
            ToTable(schema + ".VExamEventSchedule");
            HasKey(x => new { x.EventIdentifier });
        }
    }
}
