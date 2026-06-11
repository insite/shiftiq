using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TScormRegistrationConfiguration : EntityTypeConfiguration<TScormRegistration>
    {
        public TScormRegistrationConfiguration()
        {
            ToTable("TScormRegistration", "integration");
            HasKey(x => new { x.ScormRegistrationIdentifier });

            Property(x => x.LearnerIdentifier).IsRequired();
            Property(x => x.ScormRegistrationIdentifier).IsRequired();
            Property(x => x.LearnerEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.LearnerName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ScormPackageHook).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ScormRegistrationCompletion).IsUnicode(false).HasMaxLength(20);
            Property(x => x.ScormRegistrationSuccess).IsUnicode(false).HasMaxLength(20);
            Property(x => x.ScormLaunchCount).IsRequired();
            Property(x => x.ScormRegistrationInstance);
            Property(x => x.ScormRegistrationTrackedSeconds);
            Property(x => x.ScormRegistrationScore).HasPrecision(5, 4);
            Property(x => x.ScormAccessedFirst);
            Property(x => x.ScormAccessedLast);
            Property(x => x.ScormCompleted);
            Property(x => x.ScormLaunchedFirst).IsRequired();
            Property(x => x.ScormLaunchedLast).IsRequired();
            Property(x => x.ScormSynchronized);
        }
    }
}
