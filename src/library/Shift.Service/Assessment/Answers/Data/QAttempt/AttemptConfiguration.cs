using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class AttemptConfiguration : IEntityTypeConfiguration<AttemptEntity>
{
    public void Configure(EntityTypeBuilder<AttemptEntity> builder) 
    {
        builder.ToTable("QAttempt", "assessments");
        builder.HasKey(x => new { x.AttemptIdentifier });
            
        builder.Property(x => x.AttemptGraded).HasColumnName("AttemptGraded");
        builder.Property(x => x.AttemptDuration).HasColumnName("AttemptDuration").HasPrecision(12, 3);
        builder.Property(x => x.AttemptGrade).HasColumnName("AttemptGrade").IsUnicode(false).HasMaxLength(4);
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier").IsRequired();
        builder.Property(x => x.AttemptImported).HasColumnName("AttemptImported");
        builder.Property(x => x.AttemptIsPassing).HasColumnName("AttemptIsPassing").IsRequired();
        builder.Property(x => x.AttemptNumber).HasColumnName("AttemptNumber").IsRequired();
        builder.Property(x => x.AttemptPinged).HasColumnName("AttemptPinged");
        builder.Property(x => x.AttemptPoints).HasColumnName("AttemptPoints").HasPrecision(7, 2);
        builder.Property(x => x.AttemptScore).HasColumnName("AttemptScore").HasPrecision(9, 8);
        builder.Property(x => x.AttemptStarted).HasColumnName("AttemptStarted");
        builder.Property(x => x.AttemptStatus).HasColumnName("AttemptStatus").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AttemptTag).HasColumnName("AttemptTag").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.AttemptTimeLimit).HasColumnName("AttemptTimeLimit");
        builder.Property(x => x.AssessorUserIdentifier).HasColumnName("AssessorUserIdentifier").IsRequired();
        builder.Property(x => x.FormIdentifier).HasColumnName("FormIdentifier").IsRequired();
        builder.Property(x => x.FormPoints).HasColumnName("FormPoints").HasPrecision(7, 2);
        builder.Property(x => x.RegistrationIdentifier).HasColumnName("RegistrationIdentifier");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.UserAgent).HasColumnName("UserAgent").IsUnicode(false).HasMaxLength(512);
        builder.Property(x => x.AttemptSubmitted).HasColumnName("AttemptSubmitted");
        builder.Property(x => x.LearnerUserIdentifier).HasColumnName("LearnerUserIdentifier").IsRequired();
        builder.Property(x => x.GradingAssessorUserIdentifier).HasColumnName("GradingAssessorUserIdentifier");
        builder.Property(x => x.SectionsAsTabsEnabled).HasColumnName("SectionsAsTabsEnabled").IsRequired();
        builder.Property(x => x.TabNavigationEnabled).HasColumnName("TabNavigationEnabled").IsRequired();
        builder.Property(x => x.FormSectionsCount).HasColumnName("FormSectionsCount");
        builder.Property(x => x.ActiveSectionIndex).HasColumnName("ActiveSectionIndex");
        builder.Property(x => x.SingleQuestionPerTabEnabled).HasColumnName("SingleQuestionPerTabEnabled").IsRequired();
        builder.Property(x => x.ActiveQuestionIndex).HasColumnName("ActiveQuestionIndex");
        builder.Property(x => x.TabTimeLimit).HasColumnName("TabTimeLimit").IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.AttemptPingInterval).HasColumnName("AttemptPingInterval");
        builder.Property(x => x.AttemptLanguage).HasColumnName("AttemptLanguage").IsUnicode(false).HasMaxLength(2);

    }
}