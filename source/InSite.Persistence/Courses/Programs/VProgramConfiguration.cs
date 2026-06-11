using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VProgramConfiguration : EntityTypeConfiguration<VProgram>
    {
        public VProgramConfiguration()
        {
            ToTable("VProgram", "learning");
            HasKey(x => new { x.ProgramIdentifier });

            Property(x => x.AchievementIdentifier);
            Property(x => x.CompletionTaskIdentifier);
            Property(x => x.GroupIdentifier);
            Property(x => x.NotificationCompletedAdministratorMessageIdentifier);
            Property(x => x.NotificationCompletedLearnerMessageIdentifier);
            Property(x => x.NotificationStalledAdministratorMessageIdentifier);
            Property(x => x.NotificationStalledLearnerMessageIdentifier);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.AchievementElseCommand).IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementThenCommand).IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenChange).IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenGrade).IsUnicode(false).HasMaxLength(20);
            Property(x => x.GroupName).IsUnicode(false).HasMaxLength(90);
            Property(x => x.ProgramCode).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramDescription).IsUnicode(false).HasMaxLength(2147483647);
            Property(x => x.ProgramIcon).IsUnicode(false).HasMaxLength(30);
            Property(x => x.ProgramImage).IsUnicode(false).HasMaxLength(200);
            Property(x => x.ProgramName).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.ProgramSlug).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramTag).IsUnicode(false).HasMaxLength(40);
            Property(x => x.ProgramType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.NotificationStalledReminderLimit);
            Property(x => x.NotificationStalledTriggerDay);
            Property(x => x.AchievementFixedDate);
        }
    }
}
