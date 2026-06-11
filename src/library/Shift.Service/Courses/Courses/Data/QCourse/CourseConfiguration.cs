using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shift.Contract;

namespace Shift.Service.Learning;

public class CourseConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> builder)
    {
        builder.ToTable("QCourse", "courses");
        builder.HasKey(x => new { x.CourseIdentifier });

        builder.Property(x => x.CourseIdentifier).HasColumnName("CourseIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.FrameworkStandardIdentifier).HasColumnName("FrameworkStandardIdentifier");
        builder.Property(x => x.GradebookIdentifier).HasColumnName("GradebookIdentifier");
        builder.Property(x => x.CatalogIdentifier).HasColumnName("CatalogIdentifier");
        builder.Property(x => x.CompletionActivityIdentifier).HasColumnName("CompletionActivityIdentifier");
        builder.Property(x => x.CompletedToLearnerMessageIdentifier).HasColumnName("CompletedToLearnerMessageIdentifier");
        builder.Property(x => x.CompletedToAdministratorMessageIdentifier).HasColumnName("CompletedToAdministratorMessageIdentifier");
        builder.Property(x => x.StalledToLearnerMessageIdentifier).HasColumnName("StalledToLearnerMessageIdentifier");
        builder.Property(x => x.StalledToAdministratorMessageIdentifier).HasColumnName("StalledToAdministratorMessageIdentifier");
        builder.Property(x => x.CourseAsset).HasColumnName("CourseAsset").IsRequired();
        builder.Property(x => x.CourseName).HasColumnName("CourseName").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.CourseHook).HasColumnName("CourseHook").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CourseCode).HasColumnName("CourseCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CourseImage).HasColumnName("CourseImage").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.CoursePlatform).HasColumnName("CoursePlatform").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SourceIdentifier).HasColumnName("SourceIdentifier");
        builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
        builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
        builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired();
        builder.Property(x => x.Modified).HasColumnName("Modified").IsRequired();
        builder.Property(x => x.CourseIsHidden).HasColumnName("CourseIsHidden").IsRequired();
        builder.Property(x => x.CourseLabel).HasColumnName("CourseLabel").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.CourseProgram).HasColumnName("CourseProgram").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CourseLevel).HasColumnName("CourseLevel").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CourseUnit).HasColumnName("CourseUnit").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CourseDescription).HasColumnName("CourseDescription").IsUnicode(false);
        builder.Property(x => x.CourseStyle).HasColumnName("CourseStyle").IsUnicode(false);
        builder.Property(x => x.IsMultipleUnitsEnabled).HasColumnName("IsMultipleUnitsEnabled").IsRequired();
        builder.Property(x => x.CourseSequence).HasColumnName("CourseSequence");
        builder.Property(x => x.CourseIcon).HasColumnName("CourseIcon").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CourseSlug).HasColumnName("CourseSlug").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SendMessageStalledAfterDays).HasColumnName("SendMessageStalledAfterDays");
        builder.Property(x => x.SendMessageStalledMaxCount).HasColumnName("SendMessageStalledMaxCount");
        builder.Property(x => x.IsProgressReportEnabled).HasColumnName("IsProgressReportEnabled").IsRequired();
        builder.Property(x => x.OutlineWidth).HasColumnName("OutlineWidth");
        builder.Property(x => x.AllowDiscussion).HasColumnName("AllowDiscussion").IsRequired();
        builder.Property(x => x.CourseFlagColor).HasColumnName("CourseFlagColor").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.CourseFlagText).HasColumnName("CourseFlagText").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.Closed).HasColumnName("Closed");
        builder.Property(x => x.IsDisplayOverviewOnly).HasColumnName("IsDisplayOverviewOnly").IsRequired();

    }
}

public class CourseMatchConfiguration : IEntityTypeConfiguration<CourseMatch>
{
    public void Configure(EntityTypeBuilder<CourseMatch> builder)
    {
        builder.ToTable("VCourse", "courses");
        builder.HasKey(x => new { x.CourseId });
    }
}