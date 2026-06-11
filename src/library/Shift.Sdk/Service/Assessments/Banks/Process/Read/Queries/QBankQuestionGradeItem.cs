using System;

namespace InSite.Application.Banks.Read
{
    public class QBankQuestionGradeItem
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual QBankForm Form { get; set; }
        public virtual QBankQuestion Question { get; set; }
    }
}
