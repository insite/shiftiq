using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contents.Read;

namespace InSite.Persistence
{
    public class VCommentConfiguration : EntityTypeConfiguration<VComment>
    {
        public VCommentConfiguration() : this("assets") { }

        public VCommentConfiguration(string schema)
        {
            ToTable(schema + ".VComment");
            HasKey(x => x.CommentIdentifier);

            Property(x => x.AssessmentAttemptIdentifier).IsOptional();
            Property(x => x.AssessmentBankIdentifier).IsOptional();
            Property(x => x.AssessmentFieldIdentifier).IsOptional();
            Property(x => x.CommentResolvedByUserIdentifier).IsOptional();
            Property(x => x.CommentAssignedToUserIdentifier).IsOptional();
            Property(x => x.AssessmentFormIdentifier).IsOptional();
            Property(x => x.AssessmentQuestionIdentifier).IsOptional();
            Property(x => x.AssessmentSpecificationIdentifier).IsOptional();
            Property(x => x.AuthorUserIdentifier).IsRequired();
            Property(x => x.AuthorUserName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AuthorUserRole).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.BankName).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.BankTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CommentCategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.CommentSubCategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.CommentFlag).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.CommentTag).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CommentFlagged).IsOptional();
            Property(x => x.CommentIdentifier).IsRequired();
            Property(x => x.CommentIsHidden).IsRequired();
            Property(x => x.CommentIsPrivate).IsRequired();
            Property(x => x.CommentPosted).IsRequired();
            Property(x => x.CommentResolved).IsOptional();
            Property(x => x.CommentRevised).IsOptional();
            Property(x => x.CommentSubmitted).IsOptional();
            Property(x => x.CommentText).IsRequired().IsUnicode(false);
            Property(x => x.ContainerDescription).IsOptional().IsUnicode(false);
            Property(x => x.ContainerIdentifier).IsRequired();
            Property(x => x.ContainerSubtype).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EventFormat).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EventIdentifier).IsOptional();
            Property(x => x.EventStarted).IsOptional();
            Property(x => x.IssueIdentifier).IsOptional();
            Property(x => x.LogbookExperienceIdentifier).IsOptional();
            Property(x => x.LogbookIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.RegistrationIdentifier).IsOptional();
            Property(x => x.RevisorUserIdentifier).IsOptional();
            Property(x => x.TimestampCreated).IsRequired();
            Property(x => x.TimestampCreatedBy).IsOptional();
            Property(x => x.TimestampModified).IsOptional();
            Property(x => x.TimestampModifiedBy).IsOptional();
            Property(x => x.TopicUserIdentifier).IsOptional();
            Property(x => x.TrainingProviderGroupIdentifier).IsOptional();
            Property(x => x.UploadIdentifier).IsOptional();
        }
    }
}
