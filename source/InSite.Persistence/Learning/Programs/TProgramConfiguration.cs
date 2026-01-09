using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramConfiguration : EntityTypeConfiguration<TProgram>
    {
        public TProgramConfiguration() : this("records") { }

        public TProgramConfiguration(string schema)
        {
            ToTable(schema + ".TProgram");
            HasKey(x => new { x.ProgramIdentifier });

            Property(x => x.GroupIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramTag).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ProgramDescription).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.ProgramName).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.NotificationStalledAdministratorMessageIdentifier).IsOptional();
            Property(x => x.NotificationStalledLearnerMessageIdentifier).IsOptional();
            Property(x => x.NotificationCompletedAdministratorMessageIdentifier).IsOptional();
            Property(x => x.NotificationCompletedLearnerMessageIdentifier).IsOptional();
            Property(x => x.NotificationStalledReminderLimit).IsOptional();
            Property(x => x.NotificationStalledTriggerDay).IsOptional();
            Property(x => x.CompletionTaskIdentifier).IsOptional();
            Property(x => x.ProgramSlug).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramIcon).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ProgramImage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ProgramType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.AchievementElseCommand).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementFixedDate).IsOptional();
            Property(x => x.AchievementIdentifier).IsOptional();
            Property(x => x.AchievementThenCommand).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenChange).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenGrade).IsOptional().IsUnicode(false).HasMaxLength(20);

            HasOptional(x => x.Achievement).WithMany(x => x.Programs).HasForeignKey(x => x.AchievementIdentifier);
        }
    }
}