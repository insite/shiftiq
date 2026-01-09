using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankQuestionAttachmentConfiguration : EntityTypeConfiguration<QBankQuestionAttachment>
    {
        public QBankQuestionAttachmentConfiguration() : this("banks") { }

        public QBankQuestionAttachmentConfiguration(string schema)
        {
            ToTable(schema + ".QBankQuestionAttachment");
            HasKey(x => new { x.QuestionIdentifier, x.UploadIdentifier });
        }
    }
}
