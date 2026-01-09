using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;
using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class TCourseDistributionConfiguration : EntityTypeConfiguration<TCourseDistribution>
    {
        public TCourseDistributionConfiguration() : this("courses") { }

        public TCourseDistributionConfiguration(string schema)
        {
            ToTable("TCourseDistribution", schema);

            HasKey(x => x.CourseDistributionIdentifier);

            Property(x => x.DistributionStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.DistributionComment).HasMaxLength(500);


            HasRequired(x => x.Product)
                .WithMany(x => x.CourseDistributions)
                .HasForeignKey(x => x.ProductIdentifier);

            HasOptional(x => x.Course)
                .WithMany(x => x.CourseDistributions)
                .HasForeignKey(x => x.CourseIdentifier);

            HasOptional(x => x.CourseEnrollment)
                .WithMany(x => x.CourseDistributions)
                .HasForeignKey(x => x.CourseEnrollmentIdentifier);

            HasRequired(x => x.Manager)
                .WithMany(x => x.CourseDistributions)
                .HasForeignKey(x => x.ManagerUserIdentifier);
        }
    }
}
