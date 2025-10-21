using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Integration.Moodle
{
    internal class TMoodleEventConfiguration : EntityTypeConfiguration<TMoodleEvent>
    {
        public TMoodleEventConfiguration()
        {
            ToTable("TMoodleEvent", "integration");
            HasKey(x => new { x.EventIdentifier });

            Property(x => x.ActivityIdentifier).IsRequired();
            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserGuid).IsRequired();
            Property(x => x.Action).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.CallbackUrl).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.Component).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.ContextInstanceId).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.CourseId).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.Crud).IsRequired().IsUnicode(false).HasMaxLength(1);
            Property(x => x.EventData).IsRequired().IsUnicode(false).HasMaxLength(2147483647);
            Property(x => x.EventName).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.IdNumber).IsUnicode(false).HasMaxLength(255);
            Property(x => x.ObjectId).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.ObjectTable).IsRequired().IsUnicode(false).HasMaxLength(255);
            
            Property(x => x.RelatedUserId).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.ShortName).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.Target).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.Token).IsRequired().IsUnicode(false).HasMaxLength(1024);
            Property(x => x.UserId).IsRequired().IsUnicode(false).HasMaxLength(255);
            Property(x => x.Anonymous).IsRequired();
            Property(x => x.ContextId).IsRequired();
            Property(x => x.ContextLevel).IsRequired();
            Property(x => x.EduLevel).IsRequired();
            Property(x => x.EventWhen).IsRequired();
            Property(x => x.TimeCreated).IsRequired();

            Property(x => x.OtherAttemptId).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherCmiElement).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherCmiValue).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherFinalGrade);
            Property(x => x.OtherInstanceId).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherItemId).IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherLoadedContent).IsOptional().IsUnicode(false).HasMaxLength(255);
            Property(x => x.OtherOverridden);
        }
    }
}